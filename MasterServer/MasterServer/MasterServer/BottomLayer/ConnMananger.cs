using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MasterServer
{
    class ConnMananger
    {
        //监听者
        public Socket Listener;

        //连接池
        public Conn[] Conns;

        public const int MAX_CONN = 1000;

        //协议
        public ProtocolBase proto;
        //消息分发
        public HandleConnMsg handleConnMsg = new HandleConnMsg();
        public HandlePlayerEvent handlerPlayerEvent = new HandlePlayerEvent();
        public HandlePlayerMsg handlePlayerMsg = new HandlePlayerMsg();

        //单例
        public static ConnMananger Instance;

        //主定时器
        private Timer Timer = new Timer(1000);// 一秒一次
        //心跳时间
        public const long HeartBeatTime = 30;

        public ConnMananger()
        {
            Instance = this;
        }

        /// <summary>
        /// 从连接池中找到一个空闲对象，找不到则返回null
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            if (Conns == null) return -1;
            for (int i = 0; i < Conns.Length; i++)
            {
                if (Conns[i] == null)
                {
                    Conns[i] = new Conn();
                    return i;
                }
                else if(!Conns[i].IsUse)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Start(string host,int port)
        {
            //定时器
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            Timer.AutoReset = false;
            Timer.Enabled = true;
            //数据库
            //???

            Conns = new Conn[MAX_CONN];
            for (int i = 0; i < MAX_CONN; i++)
            {
                Conns[i] = new Conn();
            }
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
        private void HandleMainTimer(object sender,ElapsedEventArgs e)
        {
            HeartBeat();
            Timer.Start();  //AutoReset
        }

        private void HeartBeat()
        {
            long timeNow = Sys.GetTimeStamp();

            for (int i=0;i<Conns.Length;i++)
            {
                Conn conn = Conns[i];
                if (conn == null) continue;
                if (!conn.IsUse) continue;
                //依次判断每个连接的心跳时间间隔是否过长
                if (conn.LastTickTime < timeNow - HeartBeatTime)
                {
                    Console.WriteLine(conn.Address+"掉线！");
                    lock (conn)
                    {
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            for(int i=0;i<Conns.Length;i++)
            {
                Conn conn = Conns[i];
                if (conn == null) continue;
                if (!conn.IsUse) continue;
                lock (conn)
                {
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
                int index = GetIndex();
                if (index < 0)
                {
                    newClient.Close();
                    Console.WriteLine("连接已满");
                }
                else
                {
                    Conn newConn = Conns[index];
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
                    Console.WriteLine("收到[" + conn.Address + "]断开连接" + ex.Message);
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
            HandleMsg(conn, protocol);

            //清除已处理的消息 ???，不懂
            int count = conn.BuffCount - conn.msgLength - sizeof(Int32);
            Array.Copy(conn.ReadBuff, sizeof(Int32) + conn.msgLength, conn.ReadBuff, 0, count);
            conn.BuffCount = count; // 赋值到头，之后 重置buffCount 到已接受新的数据的长度的末尾位置
            if (conn.BuffCount > 0)
            {
                ProcessData(conn); //接着处理剩下的没有处理玩的数据
            }
        }

        private void HandleMsg(Conn conn, ProtocolBase protoBase)
        {
            string name = protoBase.GetName();  //协议名称
            string methodName = "Msg" + name;

            //连接协议分发
            if (conn.player == null || name == "HeartBeat" || name == "Logout")
            {
                MethodInfo mm = handleConnMsg.GetType().GetMethod(methodName);//反射找到该方法名
                if (mm == null)
                {
                    Console.WriteLine("HandleMsg找不到处理连接的方法" + methodName);
                    return;
                }
                Object[] obj = new object[] { conn, protoBase };
                Console.WriteLine("处理连接消息" + conn.Address);
                mm.Invoke(handleConnMsg, obj);
            }
            //角色协议分发
            else 
            {
                MethodInfo mm = handlePlayerMsg.GetType().GetMethod(methodName);
                if (mm == null)
                {
                    Console.WriteLine("HandleMsg没有处理玩家的方法" + methodName);
                    return;
                }
                Object[] obj = new object[] { conn.player, protoBase };
                Console.WriteLine("处理玩家信息" + conn.player.id + ":" + name);
                mm.Invoke(handlePlayerMsg, obj);
            }
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
            for (int i = 0; i < Conns.Length; i++)
            {
                if (!Conns[i].IsUse) continue;
                if (Conns[i].player == null) continue;
                Send(Conns[i], protocol);
            }
        }

        public void Print()
        {
            int playerCount = 0;
            Console.WriteLine("===服务器登录信息===");
            for (int i=0;i<Conns.Length;i++)
            {
                if (Conns[i] == null) continue;
                if (!Conns[i].IsUse) continue;

                if (Conns[i].player != null)
                {
                    string str = "连接" + Conns[i].Address;
                    str += "玩家id" + Conns[i].player.id;
                    playerCount++;
                    Console.WriteLine(str);
                }           
            }
            Console.WriteLine("当前在线服务器数量 " + playerCount.ToString());
        }
    }
}
