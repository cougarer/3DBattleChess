using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LobbyMgr
{
    //单例
    public static LobbyMgr Instance;

    public LobbyMgr()
    {
        Instance = this;
    }

    private Dictionary<string, LobbyServer> serverDic = new Dictionary<string, LobbyServer>();

    //根据名字获取LobbyServer
    public LobbyServer GetLobbyServer(string hostName)
    {
        if (hostName == null)
        {
            Console.WriteLine("[bug]hostName又为空了");
        }
        LobbyServer server=null;
        serverDic.TryGetValue(hostName, out server);
        return server;
    }

    //添加服务器
    public bool AddServer(Player player,string serverDesc,string mapName)
    {
        try
        {
            lock (serverDic)
            {
                LobbyServer server = new LobbyServer(player, serverDesc, mapName);

                //设置服务器参数

                serverDic[player.id] = server;

                player.tempData.status = PlayerTempData.Status.NotPrepared;
                player.tempData.hostName = player.id;
                player.tempData.isHost = true;
                player.tempData.server = server;

                return true;
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("[LobbyMgr] 创建房间失败:"+ex.Message);
            return false;
        }
    }

    //删除服务器
    public void DelServer(string hostName)
    {
        lock (serverDic)
        {
            LobbyServer server = GetLobbyServer(hostName);
            if (server == null)
            {
                return;
            }
            else
            {
                server.Host.tempData.Init();
                serverDic.Remove(hostName);
                //房间内除主机外的另一个玩家退出房间
                Player clientPlayer=null;
                foreach (Player player in server.playerDic.Values)
                {
                    if (player.id == server.Host.id)
                        continue;
                    clientPlayer = player;
                }
                if (clientPlayer != null)
                {
                    if (clientPlayer.tempData.status == PlayerTempData.Status.Prepare
                        || clientPlayer.tempData.status == PlayerTempData.Status.NotPrepared)
                    {
                        ProtocolBytes pb = new ProtocolBytes();
                        pb.AddString("KickServer");
                        pb.AddString(clientPlayer.id);
                        clientPlayer.Send(pb);
                        server.DelPlayer(clientPlayer.id);
                    }
                    /*else if (clientPlayer.tempData.status == PlayerTempData.Status.Fighting)
                    {
                        ProtocolBytes pb = new ProtocolBytes();
                        pb.AddString("GameStatus");
                        pb.AddInt(0);
                        pb.AddInt(2);
                        clientPlayer.Send(pb);
                        server.DelPlayer(clientPlayer.id);

                        clientPlayer.tempData.status = PlayerTempData.Status.None;
                        clientPlayer.data.WinTimes++;
                        Console.WriteLine(clientPlayer.id+"胜利");
                    }*/
                }
            }
        }
    }

    public void DelServerMember(string hostName,string memberName)
    {
        if (hostName == memberName) return;
        lock (serverDic)
        {
            LobbyServer server = GetLobbyServer(hostName);
            if (server == null)
            {
                return;
            }
            else
            {
                server.DelPlayer(memberName);

                if (server.Host.tempData.status == PlayerTempData.Status.NotPrepared ||
                    server.Host.tempData.status == PlayerTempData.Status.Prepare)
                {
                    //DelClient协议
                    ProtocolBytes proto = new ProtocolBytes();
                    proto.AddString("DelClient");
                    proto.AddString(memberName);
                    server.Host.Send(proto);
                }
                /*else if(server.Host.tempData.status == PlayerTempData.Status.Fighting)
                {
                    ProtocolBytes proto = new ProtocolBytes();
                    proto.AddString("GameStatus");
                    proto.AddInt(1);
                    proto.AddInt(2);
                    server.Host.Send(proto);

                    server.Host.tempData.status = PlayerTempData.Status.None;
                    server.Host.data.WinTimes++;
                    Console.WriteLine(server.Host.id + "胜利");
                }*/
            }
        }
    }

    //发送服务器列表
    public void SendServerList(Player player)
    {
        foreach (LobbyServer server in serverDic.Values)
        {
            if (server.playerDic.Count == 2) continue;
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetServerList");
            protocol.AddString(server.ServerDesc);
            protocol.AddString(server.id);
            protocol.AddInt((int)server.ServerStatus);
            player.Send(protocol);
        }
    }

    //发送服务器房主战斗资料
    public void SendHostAchieve(Player player,string hostName)
    {
        LobbyServer server = GetLobbyServer(hostName);
        if (server == null)
        {
            Console.WriteLine("[Lobby] 要发送的战斗资料不存在！");

            ProtocolBytes protoco = new ProtocolBytes();
            protoco.AddString("GetAchieve");
            protoco.AddString("房间不存在");
            protoco.AddString("不存在");
            protoco.AddInt(-1);
            protoco.AddInt(-1);
            return;
        }

        PlayerData data = server.Host.data;

        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetAchieve");
        protocol.AddString(hostName);
        protocol.AddString(server.HostMapName);
        protocol.AddInt(data.WinTimes);
        protocol.AddInt(data.FailTimes);

        player.Send(protocol);
    }

    public void LogOutInGame(Player p)
    {
        if (p.tempData.status == PlayerTempData.Status.Fighting)
        {
            p.data.FailTimes++;
            Console.WriteLine(p.id+"战败");
            LobbyServer server = p.tempData.server;
            foreach (Player player in server.playerDic.Values)
            {
                if (p.id == player.id) continue;
                ProtocolBytes pb = new ProtocolBytes();
                pb.AddString("GameStatus");
                pb.AddInt(p.tempData.isHost ? 0 : 1);
                pb.AddInt(2);
                player.Send(pb);
            }
        }
    }
}
