using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LobbyServer
{
    //服务器名
    public string ServerDesc;

    //房主名字
    public string id;  //根据房主名字确定房间ID

    public Player Host  //房主
    {
        get { return playerDic[id]; }
    }

    //地图
    public string HostMapName;

    //房间人数状态
    public int ServerStatus;

    public const int MaxPlayer = 2;

    public Dictionary<string, Player> playerDic = new Dictionary<string, Player>();

    public enum Status
    {
        Prepare,
        Fight,
    }

    public LobbyServer(string hostName)
    {
        id = hostName;

        ServerStatus = (int)Status.Prepare;
    }

    //增加玩家
    public bool AddPlayer(Player player)
    {
        lock (playerDic)
        {
            if (playerDic.Count >= MaxPlayer)
            {
                return false;
            }

            PlayerTempData tempData = player.tempData;
            tempData.server = this;
            tempData.status = PlayerTempData.Status.Prepare;

            string id = player.id;
            playerDic[id] = player;
        }
        return true;
    }

    //删除玩家
    public void DelPlayer(string id)
    {
        lock (playerDic)
        {
            if (!playerDic.ContainsKey(id))
                return;


        }
    }
}
