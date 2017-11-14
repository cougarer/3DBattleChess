using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer
{
    partial class HandleConnMsg
    {
        //心跳检测
        //协议参数：无
        public void MsgHeartBeat(Conn conn, ProtocolBase protoBase)
        {
            //心跳处理，名为HeartBeat的协议将会被分法到这个方法，从而更新心跳时间
            conn.LastTickTime = Sys.GetTimeStamp();  //更新时间戳
            Console.WriteLine("更新心跳时间"+conn.Address);
        }

        //注册
        //协议参数： str 用户名，str 密码
        //返回协议： -1表示失败，0 表示成功
        public void MsgRegister(Conn conn, ProtocolBase protoBase)
        {
            //获取数值
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);//start在调用后会指向第二个参数的起始index
            string id = protocol.GetString(start, ref start);
            string pw = protocol.GetString(start, ref start);
            string strFormat = "收到注册协议" + conn.Address;
            //构建返回协议
            protocol = new ProtocolBytes();
            protocol.AddString("Register");
            //注册
            //if(DataMgr.Instance.Register(id,pw))
            //{
            //    protocol.AddInt(0);
            //}
            //else
            //{
            //    protocol.AddInt(-1);
            //}//创建一个返回协议

            //成功就创建角色

        }
    }
}
