using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class HandlePlayerMsg
{
    //获取服务器列表
    public void MsgGetServerList(Player player, ProtocolBase protoBase)
    {
        LobbyMgr.Instance.SendServerList(player);
    }

    //得到身份资料
    //参数 string GetAchieve
    //     string 名字
    public void MsgGetAchieve(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        protocol.GetString(start, ref start);   //"GetAchieve"
        string protoHostName = protocol.GetString(start, ref start);  //取出要得到的那个名字

        LobbyMgr.Instance.SendHostAchieve(player, protoHostName);
    }

    //开房
    //协议参数：string 地图名字
    //          string 地图描述 
    //返回参数:int 1表示创建成功，0失败
    public void MsgCreateServer(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        protocol.GetString(start, ref start);   //"CreateServer"
        string protoHostMapName = protocol.GetString(start, ref start);   //地图名字
        string protoServerDesc = protocol.GetString(start, ref start);   //服务器描述

        ProtocolBytes protocolCB = new ProtocolBytes();
        protocolCB.AddString("CreateServer");
        if (LobbyMgr.Instance.AddServer(player, protoServerDesc, protoHostMapName))
        {
            protocolCB.AddInt(1);
        }
        else
        {
            protocolCB.AddInt(0);
        }
        player.Send(protocolCB);
    }

    //加入房间
    //协议参数:string 房主id
    public void MsgJoinServer(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        protocol.GetString(start, ref start);   //"JoinServer"
        string hostID = protocol.GetString(start, ref start);

        LobbyServer server = LobbyMgr.Instance.GetLobbyServer(hostID);
        ProtocolBytes infoProtocol;
        if (server.AddPlayer(player))
        {
            infoProtocol = server.GetServerPlayersInfo();
        }
        else
        {
            infoProtocol = new ProtocolBytes();

            infoProtocol.AddString("JoinServer");
            infoProtocol.AddInt(-1);
        }
        player.Send(infoProtocol);
    }
}
