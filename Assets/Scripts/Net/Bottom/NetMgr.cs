using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NetMgr
{
    public static Connection srvConn = new Connection();
    //public static Connection platformConn = new Connection();  跟平台之间的连接

    public static void Update()
    {
        srvConn.Update();
        //platformConn.Update();
    }

    //心跳协议
    public static ProtocolBase GetHeartBeatProtocol()
    {
        //具体的发送内容根据服务端设定进行改动
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("HeartBeat");
        return protocol;
    }
}

