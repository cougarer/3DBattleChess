using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : PanelBase
{
    private Button btnHostGame;
    private Button btnRefresh;
    private Button btnJoin;
    private Button btnBackToMenu;
    private Transform ServerContent;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Panel/MainMenu/LobbyPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();

        Transform skinTrans = skin.transform;
        btnHostGame = skinTrans.Find("BtnHostGame").GetComponent<Button>();
        btnRefresh = skinTrans.Find("BtnRefresh").GetComponent<Button>();
        btnJoin = skinTrans.Find("BtnJoin").GetComponent<Button>();
        btnBackToMenu = skinTrans.Find("BtnBackToMenu").GetComponent<Button>();
        ServerContent = skinTrans.Find("Scroll View/Viewport/Content");

        btnHostGame.onClick.AddListener(BtnHostGame);
        btnRefresh.onClick.AddListener(BtnRefresh);
        btnJoin.onClick.AddListener(BtnJoin);
        btnBackToMenu.onClick.AddListener(BtnBackToMenu);
    }
    #endregion

    #region 按钮监听
    private void BtnHostGame()
    {
        //保存用户名

        PanelMgr.Instance.OpenPanel<HostPanel>("");

        Close();
    }
    private void BtnRefresh()
    {
        Debug.Log("Refresh");
    }
    private void BtnJoin()
    {
        Debug.Log("Join");
    }

    private void BtnBackToMenu()
    {
        PanelMgr.Instance.OpenPanel<MenuButtonsPanel>("");
        Close();
    }
    #endregion
}
