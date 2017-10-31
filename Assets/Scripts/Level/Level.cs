using System.Collections.Generic;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Level
{
    public int Xlimit;
    public int Zlimit;

    public string LevelName;

    public List<Point> TerrainPosList;
    public List<GridType> TerrainTypeList;
    public List<SideType> TerrainSideList;

    public List<Point> UnitPosList;
    public List<GridType> UnitTypeList;
    public List<SideType> UnitSideList;

    public Level(int xlimit, int zlimit, string name)
    {
        Xlimit = xlimit;
        Zlimit = zlimit;

        LevelName = name;

        TerrainPosList = new List<Point>();
        TerrainTypeList = new List<GridType>();
        TerrainSideList = new List<SideType>();

        UnitPosList = new List<Point>();
        UnitTypeList = new List<GridType>();
        UnitSideList = new List<SideType>();
    }

    public void Clear()
    {
        TerrainPosList.Clear();
        TerrainTypeList.Clear();
        TerrainSideList.Clear();

        UnitPosList.Clear();
        UnitTypeList.Clear();
        UnitSideList.Clear();
    }

}
