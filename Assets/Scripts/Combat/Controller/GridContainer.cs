using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class GridContainer : Singletion<GridContainer> {

    public static Level level;
    public static bool isEditorMode = true;
    public static bool GameStartKey = false;

    static string terrainPrefabRoot = "Prefabs/Terrain/";
    static string unitPrefabRoot = "Prefabs/Unit/";

    public Transform TerrainLayer;
    public Transform UnitLayer;

    /// <summary>
    /// Contains all the terrain grids，buildings included
    /// </summary>
    public Dictionary<Point, TerrainBase> TerrainDic 
    {
        get
        {
            if (terrainDic == null) terrainDic = new Dictionary<Point, TerrainBase>();
            return terrainDic;
        }
        set{}
    }
    private Dictionary<Point, TerrainBase> terrainDic;

    /// <summary>
    /// Contains all the unit grids
    /// </summary>
    public Dictionary<Point, Unit> UnitDic
    {
        get 
        {
            if (unitDic == null) unitDic = new Dictionary<Point, Unit>();
            return unitDic;
        }
        set { }
    } 
    private Dictionary<Point, Unit> unitDic;

    void Awake()
    {
        TerrainLayer = GameObject.Find("MapInitPos/TerrainLayer").GetComponent<Transform>();
        UnitLayer = GameObject.Find("MapInitPos/UnitLayer").GetComponent<Transform>();
    }
	void Start ()
    {
        TerrainDic = new Dictionary<Point, TerrainBase>();
        UnitDic = new Dictionary<Point, Unit>();
    }

    /// <summary>
    /// Add new terrain grid to GridController
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <param name="sideType"></param>
    public void AddTerrain(Point pos,GridType type,SideType sideType)
    {
        #region Type Switch
        switch (type)
        {
            case GridType.Plain:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot + "Plain").GetComponent<Plain>();
                break;
            case GridType.Reef:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"Reef").GetComponent<Reef>(); 
                break;
            case GridType.Sea:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"Sea").GetComponent<Sea>(); 
                break;
            case GridType.Shoal:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"Shoal").GetComponent<Shoal>(); 
                break;
            case GridType.Woods:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"Woods").GetComponent<Woods>();
                break;
            case GridType.Mountain:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"Mountain").GetComponent<Mountain>(); 
                break;
            case GridType.Road:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"Road").GetComponent<Road>();
                break;
            case GridType.HQ:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"HQ").GetComponent<HQ>();
                break;
            case GridType.City:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"City").GetComponent<City>();
                break;
            case GridType.Factory:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"Factory").GetComponent<Factory>();
                break;
            case GridType.Shipyard:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"Shipyard").GetComponent<Shipyard>();
                break;
            case GridType.Airport:
                TerrainDic[pos] = DisplayGrid(pos, TerrainLayer, terrainPrefabRoot+"Airport").GetComponent<Airport>(); 
                break;
            default:
                throw new System.Exception(type+"non-TerrainBase Object Entered in TerrainLayer!");
        }
        #endregion

        TerrainBase tb = TerrainDic[pos];
		tb.gridID = pos;
        //设置阵营
        #region 这里破坏了框架，希望你有更好的实现
        if (isEditorMode)
		{
            if (tb.gridType == GridType.HQ)
            {
                tb.GetComponent<HQ>().AutoCheckSide();   //HQ的阵营是单独计算的
            }
            else
            {
                tb.SetSide(sideType);
            }
		}
        else    //战斗模式下，直接加载地图
		{
            tb.SetSide (sideType);
		}
        #endregion
        tb.OnInstatiate();

        //静态物体，优化batch
        if (!isEditorMode)
        {
            tb.gameObject.isStatic = true;
        }
    }

    /// <summary>
    /// Add new unit grid to GridController
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <param name="sideType"></param>
    public void AddUnit(Point pos, GridType type, SideType sideType)
    {
        #region Type Switch
        switch (type)
        { 
            case GridType.Artillery:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Artillery").GetComponent<Artillery>();
                break;
            case GridType.ATAir:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "ATAir").GetComponent<ATAir>();
                break;
            case GridType.ATAirMissile:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "ATAirMissile").GetComponent<ATAirMissile>();
                break;
            case GridType.ATInfantry:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "ATInfantry").GetComponent<ATInfantry>();
                break;
            case GridType.BattleShip:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "BattleShip").GetComponent<BattleShip>();
                break;
            case GridType.Bomber:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Bomber").GetComponent<Bomber>();
                break;
            case GridType.CannonTank:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "CannonTank").GetComponent<CannonTank>();
                break;
            case GridType.Chopter:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Chopter").GetComponent<Chopter>();
                break;
            case GridType.Destroyer:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Destroyer").GetComponent<Destroyer>();
                break;
            case GridType.Fighter:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Fighter").GetComponent<Fighter>();
                break;
            case GridType.HeavyTank:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "HeavyTank").GetComponent<HeavyTank>();
                break;
            case GridType.Infantry:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Infantry").GetComponent<Infantry>();
                break;
            case GridType.LandingShip:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "LandingShip").GetComponent<LandingShip>();
                break;
            case GridType.LightTank:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "LightTank").GetComponent<LightTank>();
                break;
            case GridType.Rockets:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Rockets").GetComponent<Rockets>();
                break;
            case GridType.Scout:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Scout").GetComponent<Scout>();
                break;
            case GridType.Submarine:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Submarine").GetComponent<Submarine>();
                break;
            case GridType.TransportChopter:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "TransportChopter").GetComponent<TransportChopter>();
                break;
            case GridType.Transporter:
                UnitDic[pos] = DisplayGrid(pos, UnitLayer, unitPrefabRoot + "Transporter").GetComponent<Transporter>();
                break;
            default:
                throw new System.Exception(type + "non-Unit Object Entered in UnitLayer!");
        }
        #endregion

        Unit u = UnitDic[pos];
        u.gridID = pos;
        u.SetSide(sideType);
        #region 初始化高度
        TerrainBase tb = TerrainDic[pos];
        if (u.transform.position != tb.transform.position + new Vector3(0, 1, 0))
        {
            u.transform.position = tb.transform.position;
            u.transform.Translate(0, 1, 0);
        }
        #endregion
        u.OnInstatiate();
    }

    /// <summary>
    /// Instantiate the grid and return the grid.gameobject
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="layer"></param>
    /// <param name="prefabRoot"></param>
    /// <returns></returns>
    GameObject DisplayGrid(Point pos,Transform layer,string prefabRoot)
    {
        GameObject go = Resources.Load<GameObject>(prefabRoot);
        GameObject gridGo = Instantiate(go, layer);
        gridGo.transform.position = new Vector3(pos.X, 0, pos.Z);
        gridGo.name = pos.ToString();
        return gridGo;
    }

    public void ExchangeUnitData(Unit u, Point targetPos)
    {
        if (u.gridID != targetPos)
        {
            UnitDic[targetPos] = u;
            UnitDic.Remove(u.gridID);
            u.gridID = targetPos;
        }
        else
        {

        }
    }
}
