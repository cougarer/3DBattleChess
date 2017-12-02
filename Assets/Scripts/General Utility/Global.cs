using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Global : MonoBehaviour {

    public static string rootName = "Global";
    private static GameObject GlobalSingletionRoot;
    public static bool isCreated = false;

    private static Global instance;
    public static Global Instance
    {
        get
        {
            if (!isCreated)
            {
                if (GlobalSingletionRoot == null)
                {
                    GlobalSingletionRoot = GameObject.Find(rootName);
                    if (GlobalSingletionRoot == null)
                    {
                        Debug.Log("创建Global根物体:" + rootName);
                        GlobalSingletionRoot = new GameObject(rootName);
                    }
                }
                if (instance == null)
                {
                    
                    instance = GlobalSingletionRoot.GetComponent<Global>();
                    if (instance == null)
                    {
                        instance = GlobalSingletionRoot.AddComponent<Global>();
                        instance.Init();
                        isCreated = true;
                    }
                }
                return instance;
            }
            else
            {
                return instance;
            }
        }
    }

    public SoundManager soundManager;
    public GameInfo gameInfo;


    private void Init()
    {
        DontDestroyOnLoad(gameObject);

        InitComponent();
    }

    private void InitComponent()
    {
        soundManager = gameObject.AddComponent<SoundManager>();

        gameInfo = new GameInfo();
        gameInfo.Init();

    }
}
