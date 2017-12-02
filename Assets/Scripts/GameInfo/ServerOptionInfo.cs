using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerOptionInfo
{
    public string ServerDesc;

    public void Init()
    {
        ServerDesc = "BattleChess Game Server";
    }
}
