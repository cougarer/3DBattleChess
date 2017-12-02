using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public int WinTimes;
    public int FailTimes;
    public string PlayerName;

    public void Init()
    {
        WinTimes = 0;
        FailTimes = 0;
        PlayerName = "Player";
    }
}
