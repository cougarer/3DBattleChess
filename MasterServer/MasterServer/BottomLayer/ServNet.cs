using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ServNet
{
    //主定时器
    System.Timers.Timer timer = new System.Timers.Timer(1000);  //1s执行一次
    //心跳时间
    public const long heartBeatTime = 180;

    //监听套接字
    public Socket listenfd;

    //客户端连接
    public Conn[] conns;

    //最大连接数
    public int maxConn = 1024;

    //单例
    public static ServNet Instance;

    public ServNet()
    {
        Instance = this;
    }

    //获取连接池索引，返回负数表示获取失败
    public int NewIndex()
    {
        if (conns == null)
        {
            return -1;
        }

        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
            {
                conns[i] = new Conn();
                return i;
            }
            else if (conns[i].isUse == false)
            {
                return i;
            }
        }
        return -1;
    }

    public void HandleMainTimer(object sender, System.Timers.ElapsedEventArgs e)
    {
        //处理心跳
        HeartBeat();
        timer.Start();   //昨天晚上到这里
    }

    //心跳
    public void HeartBeat()
    {
        Console.WriteLine("[主定时器执行]");
        long timeNow = Sys.GetTimeStamp();

        for (int i = 0; i < conns.Length; i++)
        {
            Conn conn = conns[i];
            if (conn == null) continue;
            if (!conn.isUse) continue;

            if (conn.lastTickTime < timeNow - heartBeatTime)
            {
                Console.WriteLine("[心跳引起断开连接]" + conn.GetAdress());
                lock (conn)
                    conn.Close();
            }
        }
    }

    //开启服务器
    public void Start(string host, int port)
    {
        //心跳检测定时器
        timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
        timer.AutoReset = false;
        timer.Enabled = true;

        //连接池
        conns = new Conn[maxConn];
        for (int i = 0; i < conns.Length; i++)
        {
            conns[i] = new Conn();
        }

        //socket
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //bind
        IPAddress ipAdr = IPAddress.Parse(host);
        IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
        listenfd.Bind(ipEp);

        //listen
        listenfd.Listen(maxConn);

        //Accept
        listenfd.BeginAccept(AcceptCb,null);
        Console.WriteLine("[服务器]启动");
    }

    //Accept 回调
    private void AcceptCb(IAsyncResult ar)
    {
        try
        {
            Socket socket = listenfd.EndAccept(ar);
            int index = NewIndex();

            if (index < 0)
            {
                socket.Close();
                Console.WriteLine("[警告]连接已满");
            }
            else
            {
                Conn conn = conns[i];
                conn.Init(socket);
                string adr = conn.GetAdress();
                Console.WriteLine("客户端连接 [" + adr + "] conn池ID：" + index);
                conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
                listenfd.BeginAccept(AcceptCb, null);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("AcceptCb失败：" + ex.Message);
        }
    }

    //接收回调
    private void ReceiveCb(IAsyncResult ar)
    {
        Conn conn = (Conn)ar.AsyncState;
        lock (conn)
        {
            try
            {
                int count = conn.socket.EndReceive(ar);
                //关闭信号
                if (count <= 0)
                {
                    Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开连接");
                    conn.Close();
                    return;
                }

                //数据处理
                conn.buffCount += count;
                ProcessData(conn);

                //继续接收
                conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("收到[" + conn.GetAdress() + "]断开连接" + ex.Message);
                conn.Close();
            }
        }
    }

    private void ProcessData(Conn conn)
    {
        //小于长度字节
        if (conn.buffCount < sizeof(Int32))
        {
            return;
        }

        //消息长度
        Array.Copy(conn.readBuff, conn.lenBytes, sizeof(Int32));
        conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);
        if (conn.buffCount < conn.msgLength + sizeof(Int32))
        {
            return;
        }

        //处理消息
        string str = Encoding.UTF8.GetString(conn.readBuff, sizeof(Int32), conn.msgLength);
        Console.WriteLine("收到消息 [" + conn.GetAdress() + "] " + str);

        

        //清除以处理完的信息
        int count = conn.buffCount - conn.msgLength - sizeof(Int32);
        Array.Copy(conn.readBuff, sizeof(Int32)+conn.msgLength, conn.readBuff, 0, count);
        conn.buffCount = count;
        if (conn.buffCount > 0)
        {
            ProcessData(conn);
        }
    }

    //发送消息
    public void Send(Conn conn, string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendBuff = length.Concat(bytes).ToArray();

        try
        {
            conn.socket.BeginSend(sendBuff, 0, sendBuff.Length, SocketFlags.None, null, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[发送消息]" + conn.GetAdress();+":"+ex.Message);
        }
    }
}
