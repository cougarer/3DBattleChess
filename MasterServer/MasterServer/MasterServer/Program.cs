using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private const int Port = 8080;
        private static string thisIpStr = "47.94.251.161";

        private static TcpListener Server;
        private static bool ServerStarted;
        private static List<TcpClient> Clients;

        private static List<Room> List;
        static void Main(string[] args)
        {
            List = new List<Room>();

            Clients = new List<TcpClient>();
            try
            {
                Server = new TcpListener(IPAddress.Any, Port);
                Server.Start();
                StartListening();
                ServerStarted = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Socket error:" + ex.Message);
            }

            Thread updateThread = new Thread(Update);
            updateThread.IsBackground = true;
            updateThread.Start();
            Console.WriteLine("服务器启动完毕");
            Console.WriteLine("输入/help获取更多服务器命令");
            while (true)
            {
                string code = Console.ReadLine();
                OnIncomingCmd(code);
            }
        }

        private static void OnIncomingCmd(string code)
        {
            switch (code)
            {

            }

            Console.WriteLine();
        }
        private static void StartListening()
        {
            Server.BeginAcceptTcpClient(AcceptTcpClient, Server);
        }
        private static void AcceptTcpClient(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            Clients.Add(client);
            Console.WriteLine("服务器接收到连接！");
            StartListening();
        }
        private static void Update()
        {
            while (true)
            {
                if (!ServerStarted)
                    return;

                for (int i = 0; i < Clients.Count; i++)
                {
                    if (!IsConnected(Clients[i]))
                    {
                        if (Clients[i] != null)
                        {
                            Clients[i].Close();
                            Clients.RemoveAt(i);
                        }
                    }
                    else
                    {
                        NetworkStream s = Clients[i].GetStream();
                        if (s.DataAvailable)
                        {
                            StreamReader sr = new StreamReader(s, true);
                            string data = sr.ReadLine();
                            if (data != null)
                                OnIncomingData(data, Clients[i]);
                        }
                    }
                }
            }
        }
        private static bool IsConnected(TcpClient c)
        {
            try
            {
                if (c != null && c.Client != null && c.Client.Connected)
                {
                    if (c.Client.Poll(0, SelectMode.SelectRead))
                    {
                        return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                    }
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private static void OnIncomingData(string data, TcpClient client)   //发送和接收排名信息
        {
            Console.WriteLine("接收到客户端:" + data);
            
        }
    }
    public class Room
    {
        public string HostName;
        public string IP;
        public string Map;
    }
}
