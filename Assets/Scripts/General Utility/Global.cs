using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Global : MonoBehaviour {

    public static string rootName = "Global";
    private static GameObject GlobalSingletionRoot;

    private static Global instance;
    public static Global Instance
    {
        get
        {
            if (GlobalSingletionRoot == null)
            {
                GlobalSingletionRoot = GameObject.Find(rootName);
                if (GlobalSingletionRoot == null) Debug.Log("please create a gameobject named " + rootName);
            }
            if (instance == null)
            {
                instance = GlobalSingletionRoot.GetComponent<Global>();
                if (instance == null) instance = GlobalSingletionRoot.AddComponent<Global>();
            }
            return instance;
        }
    }

    public static bool isCreated = false;

    public SoundManager soundManager;
    public static bool EditorMode = true;


    void Start ()
    {
        if (isCreated) Destroy(gameObject);
        isCreated = true;
        DontDestroyOnLoad(gameObject);

        soundManager = gameObject.AddComponent<SoundManager>();
	}
}
