using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Author: MaxLykoS
//UpdateTime: 2017/11/7

public class MainMenuMananger : MonoBehaviour
{
    void Start()
    {
        MenuButtonsPanel.gameObject.SetActive(true);
        LobbyPanel.gameObject.SetActive(false);
        HostGamePanel.gameObject.SetActive(false);
        RoomPanel.gameObject.SetActive(false);

        mapList = new List<GameObject>();
    }

    #region 主菜单四个按钮,开启大厅
    public Transform MenuButtonsPanel;

    /// <summary>
    /// 进入联机大厅
    /// </summary>
    public void BtnLobby()
    {
        MenuButtonsPanel.gameObject.SetActive(false);

        LobbyPanel.gameObject.SetActive(true);
    }

    public void BtnQuitGame()
    {
        Application.Quit();
    }

    public void BtnMapEditor()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void BtnOptions()
    {

    }
    #endregion

    #region 联机大厅
    /// <summary>
    /// 联机大厅面板
    /// </summary>
    public Transform LobbyPanel;
    public InputField NameField;
    private string PlayerName;
    public void BtnBackToMenu()
    {
        MenuButtonsPanel.gameObject.SetActive(true);

        LobbyPanel.gameObject.SetActive(false);
    }

    public void BtnRefresh()
    {

    }

    public void BttnJoin()
    {
        PlayerName = NameField.text;
        if (PlayerName == null) return;

    }

    private List<GameObject> mapList;
    /// <summary>
    /// 进入开服面板
    /// </summary>
    public void BtnHostGamePanel()
    {
        PlayerName = NameField.text;
        if (PlayerName == null) return;

        LobbyPanel.gameObject.SetActive(false);
        //中断连接

        HostGamePanel.gameObject.SetActive(true);

        //加载本地地图
        foreach (string levelName in MapLoader.LevelDic.Keys)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Lobby/LevelTag"));
            go.transform.GetComponentInChildren<Text>().text = levelName;    //关卡名
            go.GetComponent<Button>().onClick.AddListener(delegate () { BtnChooseMap(levelName); });//关卡名
            go.transform.SetParent(MapListPanel);
            mapList.Add(go);
        }
        //加载第三方地图
        foreach (string levelName in MapLoader.CustomLevelDic.Keys)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Lobby/LevelTag"));
            go.transform.GetComponentInChildren<Text>().text = levelName;    //关卡名
            go.GetComponent<Button>().onClick.AddListener(delegate () {BtnChooseMap(levelName);});//关卡名
            go.transform.SetParent(MapListPanel);
            mapList.Add(go);
        }
    }
    #endregion

    #region 开房间
    /// <summary>
    /// 开服面板
    /// </summary>
    public Transform HostGamePanel;
    /// <summary>
    /// 显示所有关卡
    /// </summary>
    public Transform MapListPanel;

    private string CurrentMapName=null;
    public void BtnHostGame()
    {

    }

    /// <summary>
    /// 返回联机大厅
    /// </summary>
    public void BtnBackToLobbyPanel()
    {
        for (int i = 0; i < mapList.Count; i++)
        {
            Destroy(mapList[i].gameObject);
        }
        HostGamePanel.gameObject.SetActive(false);
        LobbyPanel.gameObject.SetActive(true);
    }

    public void BtnChooseMap(string name)
    {
        CurrentMapName = name;
        Debug.Log(name);
    }
    #endregion

    #region 房间等待大厅
    public Transform RoomPanel;
    public void BtnStartServer()
    {

    }
    #endregion
}
