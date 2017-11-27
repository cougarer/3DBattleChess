using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : PanelBase
{
    private InputField inputFieldName;
    private InputField inputFieldPwd;
    private InputField inputFieldPwdCheck;
    private Text textPwdStrength;
    private Button btnReg;
    private Button btnBackToLoginPanel;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);

        skinPath = "Panel/MainMenu/RegisterPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();

        Transform skinTrans = skin.transform;
        inputFieldName = skinTrans.Find("InputFieldName").GetComponent<InputField>();
        inputFieldPwd = skinTrans.Find("InputFieldPwd").GetComponent<InputField>();
        textPwdStrength = inputFieldPwd.transform.Find("TextPwdStrength").GetComponent<Text>();
        inputFieldPwdCheck = skinTrans.Find("InputFieldPwdCheck").GetComponent<InputField>();
        btnReg = skinTrans.Find("BtnReg").GetComponent<Button>();
        btnBackToLoginPanel = skinTrans.Find("BtnBackToLoginPanel").GetComponent<Button>();

        textPwdStrength.text = "密码强度：弱";
        btnReg.onClick.AddListener(BtnReg);
        btnBackToLoginPanel.onClick.AddListener(BtnBackToLoginPanel);
        inputFieldPwd.onValueChanged.AddListener(OnPwdChange);
    }
    #endregion

    #region 按钮监听
    private void BtnReg()
    {
        if (inputFieldPwd.text != inputFieldPwdCheck.text)
        {
            Debug.Log("两次输入密码不一致！");
            return;
        }

        if (inputFieldName.text == "" && inputFieldPwd.text == "")
        {
            Debug.Log("用户名密码不能为空！");
            return;
        }

        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            string host = "127.0.0.1";
            int port = 1234;
            NetMgr.srvConn.proto = new ProtocolBytes();
            NetMgr.srvConn.Connect(host, port);
        }

        //发送注册信息
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Register");
        protocol.AddString(inputFieldName.text);
        protocol.AddString(inputFieldPwd.text);
        Debug.Log("发送注册申请 " + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnRegBack);
    }

    private void OnPwdChange(string pwd)
    {
        /*int pwdStrength = 0;
        bool isC = false, isc = false, isnum = false;
        foreach (char c in pwd)
        {
            if (!isC && 大写) { pwdStrength++; isC = true; }
            if (!isc && 小写) { pwdStrength++; isc = true; }
            if (!isnum&&数字) { pwdStrength++;isnum = true; }
        }

        switch (pwdStrength)
        {
            case 0:
                textPwdStrength.text = "密码强度：弱";break;
            case 1:
                textPwdStrength.text = "密码强度：弱"; break;
            case 2:
                textPwdStrength.text = "密码强度：中"; break;
            case 3:
                textPwdStrength.text = "密码强度：强"; break;
        }*/
    }

    private void BtnBackToLoginPanel()
    {
        PanelMgr.Instance.OpenPanel<LoginPanel>("");
        Close();
    }
    #endregion

    #region 网络监听
    private void OnRegBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            Debug.Log("注册成功！");
            PanelMgr.Instance.OpenPanel<LoginPanel>("");
            Close();
        }
        else
        {
            Debug.Log("注册失败!");
        }
    }
    #endregion
}
