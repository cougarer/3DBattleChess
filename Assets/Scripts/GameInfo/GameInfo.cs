using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameInfo
{
    private static string dataPath = Application.dataPath + "/Info/GameInfo.dat";

    public PlayerInfo playerInfo;
    public ServerOptionInfo serverOptionInfo;

    public void Init()
    {
        if (!Directory.Exists(Application.dataPath + "/Info"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Info");
        }

        #region 如果信息不存在，先保存出一个默认值再处理
        if (!File.Exists(dataPath))
        {
            using (FileStream fs = File.Create(dataPath))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    GameInfo defaultGameInfo = CreateDefaultGameInfo();
                    sw.WriteLine(JsonUtility.ToJson(defaultGameInfo));
                }
            }
        }
        #endregion

        #region 从文件中读取数据信息
        GameInfo gameInfo;
        using (StreamReader reader = new StreamReader(dataPath))
        {
            gameInfo = JsonUtility.FromJson<GameInfo>(reader.ReadToEnd());
        }

        playerInfo = gameInfo.playerInfo;
        serverOptionInfo = gameInfo.serverOptionInfo;
        #endregion
    }

    private GameInfo CreateDefaultGameInfo()
    {
        GameInfo info = new GameInfo();

        info.playerInfo = CreateDefaultPlayerInfo();
        info.serverOptionInfo = CreateDefaultServerOptionInfo();

        return info;
    }

    private PlayerInfo CreateDefaultPlayerInfo()
    {
        PlayerInfo info = new PlayerInfo();
        info.Init();
        return info;
    }

    private ServerOptionInfo CreateDefaultServerOptionInfo()
    {
        ServerOptionInfo info = new ServerOptionInfo();
        info.Init();
        return info;
    }
}
