using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : PanelBase
{
    private InputField inputFieldName;
    private InputField inputFieldPwd;
    private Button btnLogin;
    private Button btnReg;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);

        skinPath= "Panel/MainMenu/LoginPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();

        Transform skinTrans = skin.transform;
        inputFieldName = skinTrans.Find("InputFieldName").GetComponent<InputField>();
        inputFieldPwd = skinTrans.Find("InputFieldPwd").GetComponent<InputField>();
        btnLogin = skinTrans.Find("BtnLogin").GetComponent<Button>();
        btnReg = skinTrans.Find("BtnReg").GetComponent<Button>();

        btnLogin.onClick.AddListener(OnLoginClick);
        btnReg.onClick.AddListener(OnRegClick);
    }
    #endregion

    #region 按钮监听
    public void OnRegClick()
    {
        //PanelMgr.Instance.OpenPanel<RegPanel>("");
        Close();
    }

    public void OnLoginClick()
    {
        //用户名密码为空
        if (inputFieldName.text == null || inputFieldPwd.text == null)
        {
            Debug.Log("用户名密码不能为空");
            return;
        }

        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            string host = "127.0.0.1";
            int port = 1234;
            NetMgr.srvConn.proto = new ProtocolBytes();  //用来接受服务器发送的信息
            NetMgr.srvConn.Connect(host, port);
        }

        //发送登录申请
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(inputFieldName.text);
        protocol.AddString(inputFieldPwd.text);

        int start = 0;
        Debug.Log("发送消息" + protocol.GetString(start,ref start));
        Debug.Log("发送消息" + protocol.GetString(start, ref start));
        Debug.Log("发送消息" + protocol.GetString(start, ref start));
        NetMgr.srvConn.Send(protocol, OnLoginBack);
    }

    private void OnLoginBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);   //昨晚在这里
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            Debug.Log("登录成功");

            //进入游戏主菜单
            PanelMgr.Instance.OpenPanel<MenuButtonsPanel>("");
            Close();
        }
        else
        {
            Debug.Log("登录失败");
        }
    }

    #endregion
}
