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
        PanelMgr.Instance.OpenPanel<LoginPanel>("");
    }

    void Update()
    {
        NetMgr.Update();
    }
}
