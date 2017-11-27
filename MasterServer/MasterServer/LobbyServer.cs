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

    //房主对手名字
    public string HostOpponent;

    //观众
    public List<string> SpectList = new List<string>();

    public LobbyServer(string hostName)
    {
        HostName = hostName;
    }
}
