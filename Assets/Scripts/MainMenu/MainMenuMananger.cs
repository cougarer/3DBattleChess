using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/11/7

public class MainMenuMananger : MonoBehaviour
{
    void Start()
    {
        BtnLobbyTr = GameObject.Find("Canvas/MenuButtonsPanel/BtnLobby").transform;
        BtnMapEditorTr = GameObject.Find("Canvas/MenuButtonsPanel/BtnMapEditor").transform;
        BtnOptionsTr = GameObject.Find("Canvas/MenuButtonsPanel/BtnOptions").transform;
        BtnQuitGameTr = GameObject.Find("Canvas/MenuButtonsPanel/BtnQuit").transform;

        TrList = new List<Transform>();
        TrList.Add(BtnLobbyTr);
        TrList.Add(BtnMapEditorTr);
        TrList.Add(BtnOptionsTr);
        TrList.Add(BtnQuitGameTr);
    }

    #region 主菜单四个按钮,开启大厅
    private Transform BtnLobbyTr;
    private Transform BtnMapEditorTr;
    private Transform BtnOptionsTr;
    private Transform BtnQuitGameTr;

    public Transform LobbyPanel;

    List<Transform> TrList;   //用来存储要隐藏或显示的UI


    public void BtnLobby()
    {
        foreach (Transform tr in TrList)
        {
            tr.gameObject.SetActive(false);
        }

        LobbyPanel.gameObject.SetActive(true);
    }

    public void BtnQuitGame()
    {
        Application.Quit();
    }

    public void BtnMapEditor()
    {

    }

    public void BtnOptions()
    {

    }
    #endregion

    #region 联机大厅
    public void BtnBackToMenu()
    {
        foreach (Transform tr in TrList)
        {
            tr.gameObject.SetActive(true);
        }

        LobbyPanel.gameObject.SetActive(false);
    }

    public void Refresh()
    {

    }

    public void Join()
    {
        
    }

    public void HostGame()
    {

    }
    #endregion
}
