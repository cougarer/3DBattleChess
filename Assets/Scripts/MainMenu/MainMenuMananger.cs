using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UI.Panel;

//Author: MaxLykoS
//UpdateTime: 2017/11/7

public class MainMenuMananger : MonoBehaviour
{
    void Start()
    {
        Application.runInBackground = true;
        //PanelMgr.Instance.OpenPanel<LoginPanel>("");

        NetMgr.srvConn.proto = new ProtocolBytes();   //注意删除这个
        NetMgr.srvConn.Connect("127.0.0.1",1234);  //注意删除这个

        //发送登录申请  //注意删除这个
        ProtocolBytes protocol = new ProtocolBytes();  ////注意删除这个
        protocol.AddString("Login");//注意删除这个
        protocol.AddString("MaxLykoS");//注意删除这个
        protocol.AddString("123456");//注意删除这个
        NetMgr.srvConn.Send(protocol);//注意删除这个 

        PanelMgr.Instance.OpenPanel<MenuButtonsPanel>("");
    }

    void Update()
    {
        NetMgr.Update();//消息监听
    }
}
