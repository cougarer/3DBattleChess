using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Serv masterServer = new Serv();
            masterServer.Start("127.0.0.1", 1234);

            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        return;
                }
            }
        }      
    }
    public class Room
    {
        public string HostName;
        public string IP;
        public string Map;
    }
    public class Conn
    {
        public const int BUFFER_SIZE = 1024;

        public Socket socket;

        public bool isUse = false;

        public byte[] readBuff = new byte[BUFFER_SIZE];
        public int buffCount = 0;

        public Conn()
        {
            readBuff = new byte[BUFFER_SIZE];
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="socket"></param>
        public void Init(Socket socket)
        {
            this.socket = socket;
            isUse = true;
            buffCount = 0;
        }

        /// <summary>
        /// 缓冲区剩余字节数
        /// </summary>
        /// <returns></returns>
        public int BufferRemain()
        {
            return BUFFER_SIZE - buffCount;
        }

        /// <summary>
        /// 获取客户端地址
        /// </summary>
        /// <returns></returns>
        public string GetAddress()
        {
            if (isUse)
                return "无法获取地址";
            return socket.RemoteEndPoint.ToString();   //获取该连接的ip地址
        }


        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (!isUse)
                return;
            socket.Close();
            isUse = false;
            Console.WriteLine("断开连接"+GetAddress());
        }
    }
    public class Serv
    {
        //监听者
        public Socket Listener;

        //连接池
        public Conn[] Conns;

        //最大连接数
        public const int MAXCONN = 50;


        /// <summary>
        /// 获取连接池索引，返回负数表示获取失败
        /// </summary>
        /// <returns></returns>
        public int NewIndex()
        {
            if (Conns == null)
                return -1;
            for (int i = 0; i < Conns.Length; i++)
            {
                if (Conns[i] == null)
                {
                    Conns[i] = new Conn();
                    return i;
                }
                else if (!Conns[i].isUse)
                {
                    return -1;
                }
            }
            return -1;
        }

        /// <summary>
        /// 初始化连接池
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void Start(string host, int port)
        {
            //连接池对象申请内存
            Conns = new Conn[MAXCONN];
            for (int i = 0; i < MAXCONN; i++)
            {
                Conns[i] = new Conn();
            }

                //监听者建立监听
            Listener = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            Listener.Bind(new IPEndPoint(IPAddress.Parse(host), port));
            Listener.Listen(MAXCONN);
            Listener.BeginAccept(AcceptCb, null);       
        }

        /// <summary>
        /// 接收到客户端连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket client = Listener.EndAccept(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    client.Close();
                    Console.WriteLine("警告！连接已满");
                }
                else
                {
                    Conn conn = Conns[index];   //给连接池分配连接
                    conn.Init(client);
                    string adr = conn.GetAddress();
                    Console.WriteLine("客户端连接" + adr + "连接池ID:" + index.ToString());
                    conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BufferRemain(), SocketFlags.None,ReceiveCb, conn);  //异步接收客户端数据

                    Listener.BeginAccept(AcceptCb, null);  //再次调用实现循环
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AcceptCb 失败:" + ex.Message);
            }
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            try
            {
                //获取接收的字节数
                int count = conn.socket.EndReceive(ar);

                //关闭信号
                if (count <= 0)
                {
                    Console.WriteLine("收到" + conn.GetAddress() + "断开连接");
                    conn.Close();
                    return;
                }

                //数据处理
                string str = Encoding.UTF8.GetString(conn.readBuff, 0, count);
                Console.WriteLine("收到" + conn.GetAddress() + "数据" + str);

                //广播给其他客户端
                str = conn.GetAddress() + ":" + str;
                byte[] bytes = Encoding.Default.GetBytes(str);
                //广播
                for (int i = 0; i < Conns.Length; i++)
                {
                    if (Conns[i] == null)
                        continue;
                    if (!Conns[i].isUse)
                        continue;
                    Console.WriteLine("将消息转发给" + Conns[i].GetAddress());
                    Conns[i].socket.Send(bytes);

                    //发送完继续接收
                    conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BufferRemain(), SocketFlags.None, ReceiveCb, conn);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("收到" + conn.GetAddress() + "断开连接！");
                conn.Close();
            }
        }
    }
}
