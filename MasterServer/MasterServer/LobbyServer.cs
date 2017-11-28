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
    public string HostName;  //根据房主名字确定房间ID


    public int HostWinTimes;
    public int HostFailTimes;

    public LobbyServer(string hostName)
    {
        HostName = hostName;
    }
}
