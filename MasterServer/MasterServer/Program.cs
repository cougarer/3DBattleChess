using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //DataMgr dataMgr = new DataMgr();
            Console.WriteLine("注意，当前已移除数据库模块");
            LobbyMgr lobby = new LobbyMgr();

            ServNet servNet = new ServNet();
            servNet.proto = new ProtocolBytes();   //使用ProtocolBytes,字节流协议传输信息
            servNet.Start("0.0.0.0", 8075);

            while (true)
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "quit":
                        servNet.Close();
                        return;
                    case "print":
                        servNet.Print();
                        break;
                }
            }
        }
    }
}
