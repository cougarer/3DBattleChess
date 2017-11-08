using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author: MaxLykoS
//UpdateTime: 2017/10/22

public class EditorController : MonoBehaviour
{
    public GridType CurrentClickGridType = GridType.Plain;
    public SideType CurrentClickSideType = SideType.Neutral;

    public GameObject TerrainBuildingPanel;
    public GameObject UnitPanel;

    void Start ()
    {
        //MapLoader.LoadCustomLevel("Test");
        //GridContainer.GameStartKey = true;

        GridContainer.isEditorMode = true;   //开启编辑模式

        FindBtns();
    }

    void FindBtns()
    {
        BtnSaveMapTr = GameObject.Find("Canvas/FunctionBtns/BtnSaveMap").transform;
        BtnRandomMapTr = GameObject.Find("Canvas/FunctionBtns/BtnRandomMap").transform;
        BtnClearMapTr = GameObject.Find("Canvas/FunctionBtns/BtnClearMap").transform;
        BtnMenuTr = GameObject.Find("Canvas/FunctionBtns/BtnMenu").transform;

        MapInitPanel = GameObject.Find("Canvas/LevelIntPanel").transform;
        LevelNameInputField = MapInitPanel.Find("InputField").GetComponent<InputField>();
        LevelSizeDropdown = MapInitPanel.Find("Dropdown").GetComponent<Dropdown>();
    }

    #region 地图初始化面板
    private Transform MapInitPanel;
    private InputField LevelNameInputField;
    private Dropdown LevelSizeDropdown;
    public void BtnSaveSettings()
    {
        string levelName = LevelNameInputField.text;

        int[] size = new int[2];

        switch (LevelSizeDropdown.value)
        {
            case 0:
                size[0] = 20; size[1] = 30;break;
            case 1:
                size[0] = 16;size[1] = 25;break;
            case 2:
                size[0] = 16;size[1] = 16;break;
        }

        MapInitPanel.gameObject.SetActive(false);

        GridContainer.level = new Level(size[0], size[1], levelName);
        MapLoader.DefultTerrain(GridContainer.level);
        MapLoader.LoadLevel(GridContainer.level);

        GridContainer.GameStartKey = true;
    }
    #endregion

    #region 左下面板按钮
    private bool firstClickSideBtn = true;
    /// <summary>
    /// 玩家选取格子类型
    /// </summary>
    /// <param name="type"></param>
    public void BtnGetClickType(int type)
    {
        isDeleteMode = false;
        CurrentClickGridType = (GridType)type;
        if (SwitchToUnitPanel)
        {
            if (firstClickSideBtn)
            {
                CurrentClickSideType = SideType.Friendly;
                firstClickSideBtn = false;
            }
        }
        else
        {
            CurrentClickSideType = SideType.Neutral;
        }
    }

    /// <summary>
    /// 玩家选取阵营类型
    /// </summary>
    /// <param name="side"></param>
    public void BtnGetClickSideType(int side)
    {
        CurrentClickSideType = (SideType)side;
    }

    private bool SwitchToUnitPanel = false;
    public void BtnSwitchClickTypePanel()
    {
        isDeleteMode = false;
        firstClickSideBtn = true;

        SwitchToUnitPanel = !SwitchToUnitPanel;
        if (SwitchToUnitPanel)
        {
            UnitPanel.SetActive(true);
            TerrainBuildingPanel.SetActive(false);
            CurrentClickGridType = GridType.Infantry;
            CurrentClickSideType = SideType.Friendly;
        }
        else
        {
            TerrainBuildingPanel.SetActive(true);
            UnitPanel.SetActive(false);
            CurrentClickGridType = GridType.Plain;
            CurrentClickSideType = SideType.Neutral;
        }
    }

    private bool isDeleteMode = false;
    public void BtnDeleteUnitMode()
    {
        isDeleteMode = true;
    }
    #endregion

    #region 右上编辑器功能按钮
    private Transform BtnSaveMapTr;
    private Transform BtnClearMapTr;
    private Transform BtnRandomMapTr;
    private Transform BtnMenuTr;
    private bool ShowFunctionsBtns = false;
    /// <summary>
    /// 按钮移动动画
    /// </summary>
    /// <param name="downTrueUpFalse"></param>
    /// <returns></returns>
    IEnumerator BtnsMove(bool downTrueUpFalse)
    {
        int token = downTrueUpFalse ? -1 : 1;
        for (int i = 0; i < 25; i++)
        {
            BtnSaveMapTr.Translate(0,2*token,0);
            BtnClearMapTr.Translate(0, 4*token, 0);
            BtnRandomMapTr.Translate(0, 6*token, 0);
            BtnMenuTr.Translate(0, 8*token, 0);
            yield return new WaitForSeconds(0.01f);
        }
        yield return 0;
    }
    /// <summary>
    /// 点击按钮后显示按钮列表
    /// </summary>
    public void BtnShowFunctionsBtns()
    {
        ShowFunctionsBtns = !ShowFunctionsBtns;
        if (ShowFunctionsBtns)
        {
            StartCoroutine(BtnsMove(true));
        }
        else
        {
            StartCoroutine(BtnsMove(false));
        }
    }

    /// <summary>
    /// 保存地图按钮
    /// </summary>
    public void BtnSaveMap()
    {
        MapLoader.SaveMap(GridContainer.level);
        BtnReturnMenu();
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void BtnReturnMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }

    /// <summary>
    /// 将当前地图置空
    /// </summary>
    public void BtnClearMap()
    {
        foreach (Unit u in GridContainer.Instance.UnitDic.Values)
        {
            RemoveUnitGrid(u.gridID,false);
        }
        GridContainer.Instance.UnitDic.Clear();

        //可优化
        foreach (TerrainBase tb in GridContainer.Instance.TerrainDic.Values)
        {
            RemoveTerrainGrid(tb.gridID,false);
        }
        GridContainer.Instance.TerrainDic.Clear();

        MapLoader.DefultTerrain(GridContainer.level);
        MapLoader.LoadLevel(GridContainer.level);
    }

    /// <summary>
    /// 待实现，生成随机地图
    /// </summary>
    public void BtnRandomMap()
    {

    }
    #endregion

    /// <summary>
    /// 设置地形为其他的，并设置队伍，HQ除外
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="newType"></param>
    /// <param name="newSide"></param>
    void SetTerrain(Point pos,GridType newType,SideType newSide)
    {
        RemoveTerrainGrid(pos,true);
        GridContainer.Instance.AddTerrain(pos, newType,SideType.Neutral);
    }

    /// <summary>
    /// 删除某个地形格子
    /// </summary>
    /// <param name="pos"></param>
    void RemoveTerrainGrid(Point pos,bool dicRemove)
    {
        Destroy(GridContainer.Instance.TerrainDic[pos].gameObject);
        if(dicRemove)
            GridContainer.Instance.TerrainDic.Remove(pos);
    }

    /// <summary>
    /// 只修改地形阵营
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="newSide"></param>
    void SetTerrain(Point pos, SideType newSide)
    {
        TerrainBase tb = GridContainer.Instance.TerrainDic[pos];
        if(tb.gridType!=GridType.HQ)
        tb.SetSide(newSide);
    }

    /// <summary>
    /// 检查所设置的地形是否符合常理
    /// </summary>
    /// <param name="clickPos"></param>
    /// <returns></returns>
    bool CheckTerrainIfIsLegal(TerrainBase clickGrid)
    {
        Dictionary<Point, TerrainBase> terrainDic = GridContainer.Instance.TerrainDic;
        GridType clickType = clickGrid.gridType;
        Point clickPos = clickGrid.gridID;
        switch (CurrentClickGridType)
        {
            #region CheckShoal
            case GridType.Shoal:
                {
                    if (clickType == GridType.Plain)
                    {
                        if (terrainDic.ContainsKey(clickPos.Left()))
                        {
                            if (terrainDic[clickPos.Left()].gridType == GridType.Sea)
                            {
                                return true;
                            }
                        }
                        if (terrainDic.ContainsKey(clickPos.Right()))
                        {
                            if (terrainDic[clickPos.Right()].gridType == GridType.Sea)
                            {
                                return true;
                            }
                        }
                        if (terrainDic.ContainsKey(clickPos.Up()))
                        {
                            if (terrainDic[clickPos.Up()].gridType == GridType.Sea)
                            {
                                return true;
                            }
                        }
                        if (terrainDic.ContainsKey(clickPos.Down()))
                        {
                            if (terrainDic[clickPos.Down()].gridType == GridType.Sea)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                }
            #endregion
            #region CheckRoad
            case GridType.Road:
                {
                    if (isOnLand(clickType))
                        return true;
                    break;
                }
            #endregion
            #region CheckReef
            case GridType.Reef:
                {
                    if (clickType == GridType.Sea) return true;
                    break;
                }
            #endregion
            #region CheckHQ
            case GridType.HQ:
                {
                    if (isOnLand(clickType)&&(HQ.HQEnemy==null||HQ.HQFriendly==null)) return true;
                    break;
                }
            #endregion
            #region CheckCity
            case GridType.City:
                {
                    if (isOnLand(clickType)) return true;
                    break;
                }
            #endregion
            #region Airport
            case GridType.Airport:
                {
                    if (isOnLand(clickType)) return true;
                    break;
                }
            #endregion
            #region CheckShipyard
            case GridType.Shipyard:
                {
                    if (isOnSea(clickType)) return true;
                    break;
                }
            #endregion
            #region CheckFactory
            case GridType.Factory:
                {
                    if (isOnLand(clickType)) return true;
                    break;
                }
            #endregion
            default: return true;
        }
        return false;
    }

    /// <summary>
    /// 检查所设置的单位是否符合常理
    /// </summary>
    /// <param name="clickPos"></param>
    /// <param name="newType"></param>
    /// <returns></returns>
    bool CheckUnitIfIsLegal(Point clickPos,GridType newType)
    {
        GridType currentType = GridContainer.Instance.TerrainDic[clickPos].gridType;
        switch (newType)
        { 
            #region Check Artillery
            case GridType.Artillery:
                if (isOnLandVehicle(currentType))
                    return true;
                break;
            #endregion
            #region Check ATAir
            case GridType.ATAir:
                if (isOnLandVehicle(currentType))
                    return true;
                break;
            #endregion
            #region Check ATAirMissile
            case GridType.ATAirMissile:
                if (isOnLandVehicle(currentType))
                    return true;
                break;
            #endregion
            #region Check ATInfantry
            case GridType.ATInfantry:
                if (isOnLandInfantry(currentType))
                    return true;
                break;
            #endregion
            #region Check Battleship
            case GridType.BattleShip:
                if (isOnSeaShip(currentType))
                    return true;
                break;
            #endregion
            case GridType.Bomber:return true;
            #region Check CannonTank
            case GridType.CannonTank:
                if (isOnLandVehicle(currentType))
                    return true;
                break;
            #endregion
            case GridType.Chopter:return true;
            #region Check Destroyer
            case GridType.Destroyer:
                if (isOnSeaShip(currentType))
                    return true;
                break;
            #endregion
            case GridType.Fighter:return true;
            #region Check Heavy Tank
            case GridType.HeavyTank:
                if (isOnLandVehicle(currentType))
                    return true;
                break;
            #endregion
            #region Check Infantry
            case GridType.Infantry:
                if (isOnLandInfantry(currentType))
                    return true;
                break;
            #endregion
            #region Check Landing Ship
            case GridType.LandingShip:
                if (isOnSeaShip(currentType))
                    return true;
                break;
            #endregion
            #region Check Light Tank
            case GridType.LightTank:
                if (isOnLandVehicle(currentType))
                    return true;
                break;
            #endregion
            #region Check Rockets
            case GridType.Rockets:
                if (isOnLandVehicle(currentType))
                    return true;
                break;
            #endregion
            #region Scout
            case GridType.Scout:
                if (isOnLandVehicle(currentType))
                    return true;
                break;
            #endregion
            #region Check Submarine
            case GridType.Submarine:
                if (isOnSeaShip(currentType))
                    return true;
                break;
            #endregion
            case GridType.TransportChopter:return true;
            #region Check Transporter
            case GridType.Transporter:
                if (isOnLandVehicle(currentType))
                    return true;
                break;
            #endregion
            default: throw new System.Exception("non-Unit type exception!");
        }
        return false;
    }

    /// <summary>
    /// 判断是否为陆地上的地形
    /// </summary>
    /// <param name="ClickType"></param>
    /// <returns></returns>
    bool isOnLand(GridType ClickType)
    {
        if (ClickType != GridType.Reef
            && ClickType != GridType.Sea) return true;
        return false;
    }

    /// <summary>
    /// 判断是否为海上的地形
    /// </summary>
    /// <param name="ClickType"></param>
    /// <returns></returns>
    bool isOnSea(GridType ClickType)
    {
        if (ClickType == GridType.Sea
            || ClickType == GridType.Reef) return true;
        return false;
    }

    /// <summary>
    /// 判断载具是否符合条件
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool isOnLandVehicle(GridType type)
    {
        if (type == GridType.Mountain
            || type == GridType.Reef
            || type == GridType.Sea
            || type == GridType.Shipyard)
            return false;
        return true;
    }

    /// <summary>
    /// 判断海军是否符合条件
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool isOnSeaShip(GridType type)
    {
        if (type == GridType.Sea
            ||type==GridType.Shipyard)
            return true;
        return false;
    }

    /// <summary>
    /// 判断步兵是否符合条件
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool isOnLandInfantry(GridType type)
    {
        if (isOnLandVehicle(type)
            || type == GridType.Mountain)
            return true;
        return false;
    }

    /// <summary>
    /// 删除某个单位格子
    /// </summary>
    /// <param name="pos"></param>
    void RemoveUnitGrid(Point pos,bool dicRemove)
    {
        Destroy(GridContainer.Instance.UnitDic[pos].gameObject);
        if(dicRemove)
            GridContainer.Instance.UnitDic.Remove(pos);
    }

    /// <summary>
    /// 编辑模式下点击修改地形时的回调函数
    /// </summary>
    /// <param name="clickPos"></param>
    public void ClickChangeTerrainEventHandler(Point clickPos)
    {
        if (!SwitchToUnitPanel)  // 地形建筑面板
        {
            TerrainBase tb = GridContainer.Instance.TerrainDic[clickPos];
            if (tb.IsTerrain() &&
                (tb.gridType == CurrentClickGridType || CurrentClickSideType != SideType.Neutral))
                return;
            //是地形，同型，退出

            if (tb is Building
                && CurrentClickGridType == tb.gridType
                && CurrentClickSideType == tb.Side)
                return;
            //是建筑，同型同队，退出

            if (CurrentClickSideType == SideType.Neutral)  //点击其他地形按钮时CurrentClickSideType会归中立
            {
                if (CheckTerrainIfIsLegal(tb))
                {
                    SetTerrain(clickPos, CurrentClickGridType, CurrentClickSideType);
                }
            }
            else     //如果不是中立，说明用户想修改当前地形阵营，即只修改当前格子阵营即可,此外，不能修改HQ阵营
            {
                SetTerrain(clickPos, CurrentClickSideType);
            }
        }
        else   //单位面板 可优化
        {
            if (isDeleteMode)  //如果在删除模式，就删除
            {
                if (GridContainer.Instance.UnitDic.ContainsKey(clickPos))
                {
                    RemoveUnitGrid(clickPos,true);
                }
            }
            else  //非删除模式，就删除格子后重新加载
            {
                if (CheckUnitIfIsLegal(clickPos,CurrentClickGridType))   //Check if the grid pos is legal
                {
                    if (GridContainer.Instance.UnitDic.ContainsKey(clickPos))
                    {
                        RemoveUnitGrid(clickPos,true);
                    }
                    GridContainer.Instance.AddUnit(clickPos, CurrentClickGridType, CurrentClickSideType);
                }
            }
        }
    }

}
