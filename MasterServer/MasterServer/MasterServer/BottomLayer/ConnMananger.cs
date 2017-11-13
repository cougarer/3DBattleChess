using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer.BottomLayer
{
    class ConnMananger
    {
        //监听者
        public Socket Listener;

        //连接池
        private List<Conn> Conns;
        public Stack<int> FreeConnIndexs;
        public LinkedList<int> UsingConnIndexs;

        public const int MAX_CONN = 1000;

        //协议
        public ProtocolBase proto;

        //单例
        public static ConnMananger Instance;

        public ConnMananger()
        {
            Instance = this;
        }

        /// <summary>
        /// 从连接池中找到一个空闲对象，找不到则返回null
        /// </summary>
        /// <returns></returns>
        public Conn GetNewConn()
        {
            if (FreeConnIndexs.Count == 0)
                return null;
            int id = FreeConnIndexs.Pop();
            UsingConnIndexs.AddLast(id);
            Conns[id].ID = id;
            return Conns[id];
        }

        public void Start(string host,int port)
        {
            //定时器

            //数据库

            //初始化连接池
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //bind端口
            Listener.Bind(new IPEndPoint(IPAddress.Parse(host), port));

            //Listen监听
            Listener.Listen(MAX_CONN);

            //异步Accept
            Listener.BeginAccept(AcceptCb, null);
            Console.WriteLine("服务器启动！");
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            foreach (Conn conn in Conns)
            {
                if (!conn.IsUse)
                    continue;
                lock (conn)
                {
                    UsingConnIndexs.Remove(conn.ID);
                    FreeConnIndexs.Push(conn.ID);
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 收到连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket newClient = Listener.EndAccept(ar);
                Conn newConn = GetNewConn();
                if (newConn == null)
                {
                    newClient.Close();
                    Console.WriteLine("警告，连接已满");
                }
                else
                {
                    newConn.Init(newClient);
                    string adr = newConn.Address;
                    Console.WriteLine("客户端连接" + adr + " hashcode:" + newConn.GetHashCode());
                    newConn.BeginReceive(ReceiveCb, newConn);
                }
                Listener.BeginAccept(AcceptCb, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AcceptCb失败："+ex.Message);
            }
        }

        /// <summary>
        /// 异步接收信息
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            lock (conn)
            {
                try
                {
                    int count = conn.EndReceive(ar);

                    //关闭信号
                    if (count <= 0)
                    {
                        Console.WriteLine(conn.Address+"断开连接");
                        conn.Close();
                        return;
                    }
                    conn.BuffCount += count;
                    ProcessData(conn);

                    //继续接收
                    conn.BeginReceive(ReceiveCb, conn);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                }
            }
        }

        private void ProcessData(Conn conn)
        {
            //约定信息头是数据长度，如果缓冲长度字节数不够开头的int，说明传的不全
            if (conn.BuffCount < sizeof(Int32))
            {
                return;
            }

            //信息长度,缓冲区信息头为数据长度
            Array.Copy(conn.ReadBuff, conn.LenBytes, sizeof(Int32));
            conn.msgLength = BitConverter.ToInt32(conn.LenBytes, 0);//转换成int
            if (conn.BuffCount < conn.msgLength + sizeof(Int32))
            {
                return; //不懂
            }
            ProtocolBase protocol = proto.Decode(conn.ReadBuff, sizeof(Int32), conn.msgLength);

            //???
        }

        public void Send(Conn conn, ProtocolBase protocol)
        {
            byte[] bytes = protocol.Encode();//未知
            byte[] lengthbytes = BitConverter.GetBytes(bytes.Length);//编码后算出长度，分成长度|信息 发送
            byte[] sendbuff = lengthbytes.Concat(bytes).ToArray();
            try
            {
                conn.BeginSend(sendbuff, 0, sendbuff.Length, SocketFlags.None, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送信息" + conn.Address + ex.Message);
            }
        }

        public void Broadcast(ProtocolBase protocol)
        {
            foreach (int id in UsingConnIndexs)
            {
                Send(Conns[id], protocol);
            }
        }

        public void Print()
        {
            int playerCount = 0;
            Console.WriteLine("===服务器登录信息===");
            foreach (int id in UsingConnIndexs)
            {
                string str = "连接" + Conns[id].Address;
                Console.WriteLine(str);
                playerCount++;              
            }
            Console.WriteLine("当前在线服务器数量 " + playerCount.ToString());
        }
    }
}
