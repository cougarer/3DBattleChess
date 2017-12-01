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
        None,
        Prepare,
        Fighting,
    }

    public LobbyServer server;

    public string MapName;

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
        MapName = null;
        server = null;
    }
}
