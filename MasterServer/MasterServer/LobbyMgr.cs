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
                serverDic.Remove(hostName);
                //房间内的所有玩家退出房间
                foreach (Player player in server.playerDic.Values)
                {
                    server.DelPlayer(player.id);
                }
            }
        }
    }

    //发送服务器列表
    public void SendServerList(Player player)
    {
        foreach (LobbyServer server in serverDic.Values)
        {
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
}
