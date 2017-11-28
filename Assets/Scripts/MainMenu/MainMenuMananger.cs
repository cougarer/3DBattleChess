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
        PanelMgr.Instance.OpenPanel<MenuButtonsPanel>("");
    }

    void Update()
    {
        NetMgr.Update();//消息监听
    }
}
