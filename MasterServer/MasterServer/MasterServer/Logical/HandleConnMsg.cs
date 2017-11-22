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

            //创建角色
            //DataMgr.Instance.CreatePlayer(id);
            conn.Send(protocol);
        }

        public void MsgLogin(Conn conn,ProtocolBase protoBase)
        {
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start,ref start);
            string id = protocol.GetString(start, ref start);
            string pw = protocol.GetString(start, ref start);
            string strFormat = "收到登录协议" + conn.Address;
            Console.WriteLine(strFormat + "角色名" + id + "密码" + pw);

            //构建返回协议
            ProtocolBytes protocolRet = new ProtocolBytes();
            protocolRet.AddString("Login");

            //验证
            //if(!DataMgr.Instance.CheckPassWord(id,pw)) 验证失败，返回-1，return
            //{
            //    protocolRet.AddInt(-1);
            //    conn.Send(protocolRet);
            //    return;
            //}

            //是否已经登录（重复登录则踢掉之前那个）
            ProtocolBytes protocolLotout = new ProtocolBytes();
            protocolLotout.AddString("Logout");
            protocolLotout.AddInt(0);
            if (!Player.KickOff(id, protocolLotout))//如果踢人踢不了，就不让这个新登录的人登录
            {
                protocolRet.AddInt(-1);
                conn.Send(protocolRet);
                return;
            }

            //获取玩家数据
            //PlayerData playerData = DataMgr.Instance.GetPlayerData(id);
            //if(playerData==null)
            //{
            //    protocolRet.AddInt(-1);
            //    conn.Send(protocolRet);
            //    return;
            //} 

            //conn.player = new Player(id, conn);
            //conn.player.Data = playerData;

            //事件触发
            ConnMananger.Instance.handlerPlayerEvent.OnLogin(conn.player);
            //返回协议
            protocolRet.AddInt(0);  //0为登录成功
            conn.Send(protocolRet);
        }
        //下线
        //协议参数：无
        //返回协议：0 正常下线
        public void MsgLogout(Conn conn, ProtocolBase protoBase)
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Logout");
            protocol.AddInt(0);
            conn.Send(protocol);
            if (conn.player != null)
            {
                conn.player.Logout();
            }
        }
    }
}
