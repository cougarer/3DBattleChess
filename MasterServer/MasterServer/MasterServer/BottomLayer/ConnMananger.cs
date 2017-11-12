using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer.BottomLayer
{
    class ConnMananger
    {
        //监听者
        public Socket Listener;

        //对象池
        public Stack<Conn> FreeConns;
        public Stack<Conn> UsingConns;

        public const int MAX_CONN = 1000;

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
            if (FreeConns.Count == 0)
                return null;
            Conn conn = FreeConns.Pop();
            UsingConns.Push(conn);
            return conn;
        }

        public void Start(string host,int port)
        {

        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            foreach (Conn conn in UsingConns)
            {
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
                Conn newConn = GetNewConn();
                if (newConn == null)
                    Console.WriteLine("警告，连接已满");
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
                Console.WriteLine(ex.Message);
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
                    }
                    conn.BuffCount += count;
                    ProcessData(conn);

                    //继续接收
                    conn.BeginReceive(ReceiveCb, conn);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void ProcessData(Conn conn)
        {

        }
    }
}
