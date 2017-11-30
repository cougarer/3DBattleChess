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

    public void MsgGetAchieve(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        protocol.GetString(start, ref start);
        string protoHostName = protocol.GetString(start, ref start);  //取出要得到的那个名字

        LobbyMgr.Instance.SendHostAchieve(player, protoHostName);
    }

    //开房
    //协议参数：地图名字
    public void MsgCreateServer(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        protocol.GetString(start, ref start);   //"CreateServer"
        string protoHostMapName = protocol.GetString(start, ref start);   //地图名字

        LobbyMgr.Instance.AddServer(player.id);
    }
}
