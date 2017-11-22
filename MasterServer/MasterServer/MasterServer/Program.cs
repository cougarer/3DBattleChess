using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MasterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnMananger connMananger = new ConnMananger();
            connMananger.proto = new ProtocolBase();
            connMananger.Start("127.0.0.1", 1234);

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
}
