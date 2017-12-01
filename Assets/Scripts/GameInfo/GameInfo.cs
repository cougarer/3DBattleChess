using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameInfo
{
    public PlayerInfo playerInfo;
    public ServerOptionInfo serverOptionInfo;

    private string dataPath = Application.dataPath + "/GameInfo.dat";

    public GameInfo()
    {
        DirectoryInfo

        GameInfo gameInfo;
        using (FileStream GameInfoTextStream = new FileStream(Application.dataPath, FileMode.Open))
        {
            using (StreamReader reader = new StreamReader(GameInfoTextStream))
            {
                 gameInfo =  JsonUtility.FromJson<GameInfo>(reader.ReadToEnd());           
            }
        }

        playerInfo = gameInfo.playerInfo;
        serverOptionInfo = gameInfo.serverOptionInfo;
    }
}
