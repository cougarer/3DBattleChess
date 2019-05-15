using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class HandlePlayerMsg
{
    #region 大厅同步
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
    //返回参数：
    //        给客机：JoinServer 房间人数 每个人的名字和准备状态
    //        给房主：AddClient  客机名字 准备状态
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

        ProtocolBytes hostInfoProto = new ProtocolBytes();
        hostInfoProto.AddString("AddClient");
        hostInfoProto.AddString(player.id);
        hostInfoProto.AddInt((int)player.tempData.status);
        server.Host.Send(hostInfoProto);
    }

    //离开房间，客机
    //参数：LeaveServer，离开的客机名
    public void MsgLeaveServer(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        protocol.GetString(start, ref start);   //"LeaveServer"
        string clientName = protocol.GetString(start, ref start);
        LobbyMgr.Instance.DelServerMember(player.tempData.hostName,player.id);
    }

    //关闭房间，主机
    //参数：DelServer，关闭的主机名
    public void MsgDelServer(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        protocol.GetString(start, ref start);   //"DelServer"
        string hostName = protocol.GetString(start, ref start);
        LobbyMgr.Instance.DelServer(hostName);
    }

    //中转地图文件开头
    //参数:MapPrepare 文件个数
    public void MsgMapPrepare(Player player, ProtocolBase protoBase)
    {
        foreach (Player p in LobbyMgr.Instance.GetLobbyServer(player.id).playerDic.Values)
        {
            if (p.id == player.id) continue;  //不发给自己
            //发送地图文件开头 
            p.Send((ProtocolBytes)protoBase);
        }
    }

    //中转地图数据文件
    //参数:MapData 地图文件
    public void MsgMapData(Player player, ProtocolBase protoBase)
    {
        foreach (Player p in LobbyMgr.Instance.GetLobbyServer(player.id).playerDic.Values)
        {
            if (p.id == player.id) continue;  //不发给自己
            //发送地图 
            p.Send((ProtocolBytes)protoBase);
        }
    }

    //转发大厅准备状态
    //参数:LobbyStatus 玩家名 准备状态
    public void MsgLobbyStatus(Player player, ProtocolBase protoBase)
    {
        int start = 0;
        ProtocolBytes info = (ProtocolBytes)protoBase;
        info.GetString(start, ref start);//LobbyStatus
        info.GetString(start, ref start);
        player.tempData.status = (PlayerTempData.Status)info.GetInt(start, ref start);
        if (player.tempData.status == PlayerTempData.Status.Fighting && player.tempData.isHost == false)//客机玩家发送的进入游戏指令无需广播
            return;
        LobbyServer server = player.tempData.server;
        if (server != null)
        {
            foreach (Player p in server.playerDic.Values)
            {
                p.Send(protoBase);
            }
            if (player.tempData.status == PlayerTempData.Status.Fighting)
                server.ServerStatus = LobbyServer.Status.Fight;
        }
    }
    #endregion

    #region 游戏同步
    //单位移动
    //UnitMove 部队坐标 目标坐标
    public void MsgUnitMove(Player player, ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        foreach (Player p in server.playerDic.Values)
        {
            p.Send((ProtocolBytes)protoBase);
        }

        //player.Send(protoBase);
    }

    /*游戏状态
    //GameStatus 玩家号0/1 状态
    public enum GameStatus
    {
        Ready,  //0
        Win,    //1
        Lose,   //2
    };
    //输赢本地运算，自己只发自己的
     */
    public void MsgGameStatus(Player player,ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        if (server != null)
        {
            foreach (Player p in server.playerDic.Values)
            {
                p.Send((ProtocolBytes)protoBase);
            }
        }

        ProtocolBytes info = (ProtocolBytes)protoBase;
        int start = 0;
        info.GetString(start, ref start);//GameStatus
        int id = info.GetInt(start,ref start);
        int _id = player.tempData.isHost ? 0 : 1;
        int status = info.GetInt(start, ref start);
        if (status == 1)
        {
            player.data.WinTimes++;
            Console.WriteLine(player.id + "胜利");
            player.tempData.status = PlayerTempData.Status.None;
            LobbyMgr.Instance.DelServer(player.tempData.hostName);
        }
        else if (status == 2)
        {
            player.data.FailTimes++;
            Console.WriteLine(player.id + "战败");
            player.tempData.status = PlayerTempData.Status.None;
            LobbyMgr.Instance.DelServer(player.tempData.hostName);
        }
    }

    //部队移动停止
    //MoveDone 部队位置
    public void MsgMoveDone(Player player, ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        foreach (Player p in server.playerDic.Values)
        {
            if (p.id == player.id)
                continue;//本地处理,不发给自己
            p.Send((ProtocolBytes)protoBase);
        }
    }

    //创建部队
    //CreateUnit 部队类型 位置
    public void MsgCreateUnit(Player player, ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        foreach (Player p in server.playerDic.Values)
        {
            p.Send((ProtocolBytes)protoBase);
        }
    }

    //主动攻击
    //AttackInitiative 攻击者坐标 被攻击坐标
    public void MsgAttackInitiative(Player player, ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        foreach (Player p in server.playerDic.Values)
        {
            p.Send((ProtocolBytes)protoBase);
        }
    }

    //被动攻击
    //AttackPassive 攻击者坐标 被攻击坐标
    public void MsgAttackPassive(Player player, ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        foreach (Player p in server.playerDic.Values)
        {
            p.Send((ProtocolBytes)protoBase);
        }
    }

    //销毁单位
    //UnitDestroy 被销毁单位坐标
    public void MsgUnitDestroy(Player player, ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        foreach (Player p in server.playerDic.Values)
        {
            p.Send((ProtocolBytes)protoBase);
        }
    }

    //占领建筑
    //BuildingCapture 坐标
    public void MsgBuildingCapture(Player player, ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        foreach (Player p in server.playerDic.Values)
        {
            p.Send((ProtocolBytes)protoBase);
        }
    }

    //装载单位
    //LoadUnit 部队 载具
    public void MsgLoadUnit(Player player, ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        foreach (Player p in server.playerDic.Values)
        {
            p.Send((ProtocolBytes)protoBase);
        }
    }

    //卸载单位
    //UnloadUnit 部队 载具
    public void MsgUnloadUnit(Player player, ProtocolBase protoBase)
    {
        LobbyServer server = player.tempData.server;
        foreach (Player p in server.playerDic.Values)
        {
            p.Send((ProtocolBytes)protoBase);
        }
    }
    #endregion
}
