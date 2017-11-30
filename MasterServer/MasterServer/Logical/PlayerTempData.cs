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
        Nonde,
        Prepare,
        Fighting,
    }

    public LobbyServer server;

    public bool isHost;

    public PlayerTempData()
    {
        isHost = false;
    }
}
