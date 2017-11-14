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
