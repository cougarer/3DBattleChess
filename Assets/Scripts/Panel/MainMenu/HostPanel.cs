using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostPanel : PanelBase
{
    private Button btnHostGame;
    private Button btnBackToLobby;
    private Transform MapContent;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);

        skinPath = "Panel/MainMenu/HostPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();

        Transform skinTrans = skin.transform;
        btnHostGame = skinTrans.Find("BtnHostGame").GetComponent<Button>();
        btnBackToLobby = skinTrans.Find("BtnBackToLobby").GetComponent<Button>();
        MapContent = skinTrans.Find("Scroll View/Viewport/Content");

        btnHostGame.onClick.AddListener(BtnHostGame);
        btnBackToLobby.onClick.AddListener(BtnBackToLobbyPanel);
    }

    public override void OnShowed()
    {
        base.OnShowed();

        LoadMap();
    }

    #endregion

    private void LoadMap()
    {
        //加载本地地图
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Lobby/LevelTag"));
        foreach (string levelName in MapLoader.LevelDic.Keys)
        {            
            go.transform.GetComponentInChildren<Text>().text = levelName;    //关卡名
            go.GetComponent<Button>().onClick.AddListener(delegate () { BtnChooseMap(levelName); });//关卡名
            go.transform.SetParent(MapContent);
        }
        //加载第三方地图
        foreach (string levelName in MapLoader.CustomLevelDic.Keys)
        {
            go.transform.GetComponentInChildren<Text>().text = levelName;    //关卡名
            go.GetComponent<Button>().onClick.AddListener(delegate () { BtnChooseMap(levelName); });//关卡名
            go.transform.SetParent(MapContent);
        }
    }

    #region 按钮监听
    private void BtnHostGame()
    {
        PanelMgr.Instance.OpenPanel<RoomPanel>("");
        Close();
    }

    private void BtnBackToLobbyPanel()
    {
        PanelMgr.Instance.OpenPanel<LobbyPanel>("");
        Close();
    }

    private void BtnChooseMap(string name)
    {
        Debug.Log(name);
    }
    #endregion
}
