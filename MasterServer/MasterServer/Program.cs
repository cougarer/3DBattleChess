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
            ServNet servNet = new ServNet();
            servNet.proto = new ProtocolBytes();   //使用ProtocolBytes,字节流协议传输信息
            servNet.Start("127.0.0.1", 1234);

            while (true)
            {
                string cmd = Console.ReadLine();
            }
        }
    }
}
