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
            servNet.Start("127.0.0.1", 1234);

            while (true)
            {
                string cmd = Console.ReadLine();
            }
        }
    }
}
