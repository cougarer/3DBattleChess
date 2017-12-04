using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerTempData
{
    public Status status;
    public enum Status
    {
        None,//未加入任何房间
        NotPrepared,   //加入房间了但处于未准备状态
        Prepare,//加入房间了并处于准备状态
        Fighting,    //加入房间了并处于游戏战斗状态
    }

    public LobbyServer server;

    public bool isHost;

    public PlayerTempData()
    {
        isHost = false;

        status = Status.None;
    }

    public void Init()
    {
        isHost = false;
        status = Status.None;
        server = null;
    }
}
