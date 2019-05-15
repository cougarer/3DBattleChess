using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/22

static class MapLoader
{
    public static string LevelsDirectoryPath = Application.dataPath + "/Levels/";  //    Assets/Levels/
    public static string LevelsListPath = LevelsDirectoryPath + "LevelsList.txt";  //    Assets/Levels/LevelsList.txt

    /// <summary>
    /// Contains all the offical maps
    /// </summary>
    public static Dictionary<string, Level> LevelDic
    {
        get
        {
            if (levelDic == null) levelDic = new Dictionary<string, Level>();

            if (levelDic.Count == 0) LoadAllLevels();  //判空重载
            return levelDic;
        }
    }
    private static Dictionary<string, Level> levelDic;

    /// <summary>
    /// Contains all the custom maps
    /// </summary>
    public static Dictionary<string, Level> CustomLevelDic
    {
        get
        {
            if (customLevelDic == null) customLevelDic = new Dictionary<string, Level>();

            if (customLevelDic.Count == 0) LoadAllCustomLevels();   //判空重载
            return customLevelDic;
        }
    }
    private static Dictionary<string, Level> customLevelDic;

    /// <summary>
    /// 载入所有的官方地图
    /// </summary>
    public static void LoadAllLevels()
    {
        TextAsset[] levelText = Resources.LoadAll<TextAsset>("Levels");
        foreach (TextAsset ta in levelText)
        {
            levelDic[ta.name] = JsonUtility.FromJson<Level>(ta.text);
        }
    }

    /// <summary>
    /// 载入所有的自定义地图
    /// </summary>
    public static void LoadAllCustomLevels()
    {
        if (!Directory.Exists(LevelsDirectoryPath))
        {
            Directory.CreateDirectory(LevelsDirectoryPath);
            File.CreateText(LevelsListPath).Close();
            return;
        }

        HashSet<string> levelNameSet = new HashSet<string>();
        using (StreamReader sr = new StreamReader(LevelsListPath))
        {
            string levelName;
            while ((levelName = sr.ReadLine()) != null)
            {
                string levelFilePath = LevelsDirectoryPath + levelName + ".txt";
                if (File.Exists(levelFilePath))  //存在就读取，并加入集合，不存在就不读
                {
                    levelNameSet.Add(levelName);
                    using (StreamReader srJson = new StreamReader(levelFilePath))
                    {
                        customLevelDic[levelName] = JsonUtility.FromJson<Level>(srJson.ReadToEnd());
                    }
                }
            }
        }
        using (StreamWriter sw = new StreamWriter(LevelsListPath))
        {
            foreach (string name in levelNameSet)
            {
                sw.WriteLine(name);
            }
        }
    }

    /// <summary>
    /// 将Level对象转换成默认地形
    /// </summary>
    /// <param name="level"></param>
    public static void DefultTerrain(Level level)
    {
        level.Clear();
        for (int i = 0; i < level.Xlimit; i++)
        {
            for (int j = 0; j < level.Zlimit; j++)
            {
                GridContainer.Instance.AddTerrain(new Point(i, j), GridType.Plain,SideType.Neutral);
            }
        }
    }

    /// <summary>
    /// 向自定义关卡路径中保存当前地图
    /// </summary>
    /// <param name="level"></param>
    public static void SaveMap(Level level,bool editorMode)
    {
        if (editorMode)
        {
            if (level.Xlimit < 16 || level.Zlimit < 16)
            {
                throw new Exception("Map size cant be smaller than 16 !");
            }

            //Read the data in GridController.instance's three Dictionaries
            foreach (KeyValuePair<Point, TerrainBase> kvp in GridContainer.Instance.TerrainDic)
            {
                level.TerrainPosList.Add(kvp.Key);
                level.TerrainTypeList.Add(kvp.Value.gridType);
                level.TerrainSideList.Add(kvp.Value.Side);
            }

            foreach (KeyValuePair<Point, Unit> kvp in GridContainer.Instance.UnitDic)
            {
                level.UnitPosList.Add(kvp.Key);
                level.UnitTypeList.Add(kvp.Value.gridType);
                level.UnitSideList.Add(kvp.Value.Side);
            }
        }
        //if the file doesn't exist then create one
        if (!Directory.Exists(LevelsDirectoryPath))
        {
            Directory.CreateDirectory(LevelsDirectoryPath);
        }

        using (FileStream fs = new FileStream(LevelsDirectoryPath + level.LevelName + ".txt", FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(JsonUtility.ToJson(level));
            }
        }

        using (StreamWriter sw = new StreamWriter(LevelsListPath,true))
        {
            sw.WriteLine(level.LevelName);
        }
    }

    /// <summary>
    /// 载入官方地图
    /// </summary>
    /// <param name="levelName"></param>
    public static void LoadLevel(string levelName)
    {
        Level level = LevelDic[levelName];

        LoadLevel(level);
    }
    /// <summary>
    /// 载入自定义地图
    /// </summary>
    /// <param name="levelName"></param>
    public static void LoadCustomLevel(string levelName)
    {
        Level level = CustomLevelDic[levelName];

        LoadLevel(level);
    }
    /// <summary>
    /// load one map
    /// </summary>
    /// <param name="level"></param>
    public static void LoadLevel(Level level)
    {
        for (int i = 0; i < level.TerrainPosList.Count; i++)
        {
            GridContainer.Instance.AddTerrain(level.TerrainPosList[i], level.TerrainTypeList[i],level.TerrainSideList[i]);
        }

        for (int i = 0; i < level.UnitPosList.Count; i++)
        {
            GridContainer.Instance.AddUnit(level.UnitPosList[i], level.UnitTypeList[i],level.UnitSideList[i]);
        }
        CameraController.SetXZLimit(level.Xlimit, level.Zlimit);
        GridContainer.level = level;
    }

    public static Level GetLevel(string _levelName)
    {
        Level level;
        if (customLevelDic.TryGetValue(_levelName, out level))
            return level;
        if (levelDic.TryGetValue(_levelName, out level))
            return level;
        PanelMgr.Instance.OpenPanel<UI.Tip.WarningTip>("", "找不到该地图文件！");
        throw new Exception("找不到该地图文件！");
    }
}
