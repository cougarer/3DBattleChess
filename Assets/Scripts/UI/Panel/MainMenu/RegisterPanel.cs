using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Panel;
using UI.Tip;
using System.Text.RegularExpressions;

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

        textPwdStrength.text = "";
        btnReg.onClick.AddListener(BtnReg);
        btnBackToLoginPanel.onClick.AddListener(BtnBackToLoginPanel);
        inputFieldPwd.onValueChanged.AddListener(OnPwdChange);
    }
    #endregion

    #region 按钮监听
    private void BtnReg()
    {
        if (!isLegal)
        {
            PanelMgr.Instance.OpenPanel<WarningTip>("", "密码不符合要求！");
            return;
        }

        if (inputFieldName.text == "" || inputFieldPwd.text == "")
        {
            PanelMgr.Instance.OpenPanel<WarningTip>("", "用户名密码不能为空！");
            return;
        }

        if (inputFieldPwd.text != inputFieldPwdCheck.text)
        {
            PanelMgr.Instance.OpenPanel<WarningTip>("", "两次密码输入不一致！");
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

    private bool isLegal;
    private void OnPwdChange(string pwd)
    {
        #region 密码不能为空
        if (pwd == "")
        {
            textPwdStrength.text = "";
            return;
        }
        #endregion

        #region 密码不能过长
        if (pwd.Length > 8)
        {
            isLegal = false;
            textPwdStrength.text = "密码最长为8位！";
            return;
        }
        #endregion

        bool isC = false, isc = false, isnum = false;
        isLegal =true;

        int pwdStrength = 0;

        #region 判断密码强度
        foreach (char c in pwd)
        {
            string str = c.ToString();
            #region 包含大写
            if (Regex.IsMatch(str, "[A-Z]"))
            {
                if (!isC)
                {
                    pwdStrength++;
                    isC = true;
                }
            }
            #endregion
            #region 包含小写
            else if (Regex.IsMatch(str, "[a-z]"))
            {
                if (!isc)
                { 
                    pwdStrength++;
                    isc = true;
                }
            }
            #endregion
            #region 包含数字
            else if (Regex.IsMatch(str, "[0-9]"))
            {
                if (!isnum)
                {
                    pwdStrength++;
                    isnum = true;
                }
            }
            #endregion
            #region 包含非法字符
            else
                isLegal = false;
            #endregion
        }
        #endregion

        if (!isLegal)
        {
            textPwdStrength.text = "含有非法字符";
            return;
        }
        #region 密码强度判断
        switch (pwdStrength)
        {
            case 1:
                textPwdStrength.text = "密码强度：弱"; break;
            case 2:
                textPwdStrength.text = "密码强度：中"; break;
            case 3:
                textPwdStrength.text = "密码强度：强"; break;
        }
        #endregion
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
