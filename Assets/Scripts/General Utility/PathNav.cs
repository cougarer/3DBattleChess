using System;
using System.Collections.Generic;

//Author: MaxLykoS
//UpdateTime: 2017/10/28

public static class PathNav
{
    public delegate void OnMoveEnd();

    public static Unit CurrentMovingUnit
    {
        get
        {
            return GridContainer.Instance.UnitDic[startPos];
        }
        set
        {
            startPos = value.gridID;
        }
    }
    public static HashSet<Point> ReachablePoints
    {
        get
        {
            return ReachablePointSet;
        }
    }
    /// <summary>
    /// 给单位移动动画协程设置的一个信号量，用于进入下一回合时跳过单位移动动画
    /// </summary>
    public static bool bMoving = false;

    //辅助停止单位高亮
    private static Point startPos;

    private static HashSet<Node> ReachableNodeSet = new HashSet<Node>();
    private static List<Node> UnknownList = new List<Node>();
    private static HashSet<Point> ReachablePointSet = new HashSet<Point>();

    //标记StopHighLight是否需要执行 
    private static bool isShowing = false;

    /// <summary>
    /// 显示移动范围
    /// </summary>
    /// <param name="start"></param>
    public static void ShowUnitMoveRange(Unit u)
    {
        Dictionary<Point, Node> MapDic = new Dictionary<Point, Node>();

        ReachableNodeSet.Clear();
        UnknownList.Clear();
        ReachablePointSet.Clear();

        startPos = u.gridID;//帮助停止单位高亮

        isShowing = true;

        Node.startUnit = u;

        #region 得到最大移动距离所构成的一个矩形的四个点
        int xLimit = GridContainer.level.Xlimit;
        int zLimit = GridContainer.level.Zlimit;
        int x, z;
        z = u.Oil >= startPos.Z ? 0 : startPos.Z - u.Oil;
        x = xLimit - startPos.X < u.Oil ? xLimit : startPos.X + u.Oil;
        Point TopRightCorner = new Point(x, z);

        x = startPos.X < u.Oil ? 0 : startPos.X - u.Oil;
        Point LowerRightCorner = new Point(x, z);

        z = zLimit - startPos.Z < u.Oil ? zLimit : startPos.Z + u.Oil;
        Point LowerLeftCorner = new Point(LowerRightCorner.X, z);
        //得到一个长方形，只需要遍历这个长方形中的格子用A星判断是否能到达即可
        #endregion

        #region 计算每个格子的g和h和type，得到地图
        for (int i = LowerRightCorner.X; i < TopRightCorner.X; i++) //x
        {
            for (int j = LowerRightCorner.Z; j < LowerLeftCorner.Z; j++)    //z
            {
                Point pos = new Point(i, j);

                int nodeOilCost = GridContainer.Instance.TerrainDic[pos].OilCost;
                GridType type = GridContainer.Instance.TerrainDic[pos].gridType;

                MapDic[pos] = new Node(pos, nodeOilCost, type);
            }
        }//计算每个格子的g和h,type，得到地图
        #endregion

        //借助这个位置，从这里出发， 向周围扩散
        AStarCheckRange(startPos, u.Oil, ref MapDic);
    }
    /// <summary>
    /// 求出移动范围
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private static void AStarCheckRange(Point startPos, int oilTotal, ref Dictionary<Point, Node> mapDic)
    {

        Node startNode = mapDic[startPos];

        startNode.oilLeft = oilTotal;
        startNode.parent = startNode;
        startNode.oilCost = 0;
        UnknownList.Add(startNode);
        startNode.oilLeft = startNode.parent.oilLeft - startNode.oilCost;   //求出油量剩余 ，循环

        while (UnknownList.Count != 0)
        {
            Node reachableNode = UnknownList[0];
            Node parent = UnknownList[0].parent;
            reachableNode.oilLeft = parent.oilLeft - reachableNode.gCost;

            if (reachableNode.oilLeft >= 0)  //如果可以到达
            {
                ReachableNodeSet.Add(reachableNode);  //放到可以消除的列表中

                #region 找到周围四个点并加入判断列表
                Node upNode;
                if (mapDic.TryGetValue(reachableNode.pos.Up(), out upNode))
                {
                    if (!ReachableNodeSet.Contains(upNode) && !UnknownList.Contains(upNode))
                    {
                        upNode.parent = reachableNode;
                        UnknownList.Add(upNode);
                    }
                }

                Node rightNode;
                if (mapDic.TryGetValue(reachableNode.pos.Right(), out rightNode))
                {
                    if (!ReachableNodeSet.Contains(rightNode) && !UnknownList.Contains(rightNode))
                    {
                        rightNode.parent = reachableNode;
                        UnknownList.Add(rightNode);
                    }
                }

                Node downNode;
                if (mapDic.TryGetValue(reachableNode.pos.Down(), out downNode))
                {
                    if (!ReachableNodeSet.Contains(downNode) && !UnknownList.Contains(downNode))
                    {
                        downNode.parent = reachableNode;
                        UnknownList.Add(downNode);
                    }
                }

                Node leftNode;
                if (mapDic.TryGetValue(reachableNode.pos.Left(), out leftNode))
                {
                    if (!ReachableNodeSet.Contains(leftNode) && !UnknownList.Contains(leftNode))
                    {
                        leftNode.parent = reachableNode;
                        UnknownList.Add(leftNode);
                    }

                }
                #endregion
            }

            UnknownList.RemoveAt(0);
        }
        foreach (Node node in ReachableNodeSet)
        {
            GridContainer.Instance.TerrainDic[node.pos].SetHighLight();

            ReachablePointSet.Add(node.pos);
        }
    }

    /// <summary>
    /// 停止显示移动范围
    /// </summary>
    public static void StopShowUnitMoveRange()
    {
        if (isShowing)
        {
            foreach (Node node in ReachableNodeSet)
            {
                GridContainer.Instance.TerrainDic[node.pos].StopHighLight();
            }
            GridContainer.Instance.UnitDic[startPos].StopHighLight();

            isShowing = false;
        }
    }
}
public class Node
{
    public static Unit startUnit;

    //当前位置的地形类型
    public GridType terrainType;

    //坐标
    public Point pos;

    public int gCost  //当前格子的oilCost
    {
        get
        {
            if (GridContainer.Instance.UnitDic.ContainsKey(pos) && pos != startUnit.gridID)   //这里有一个单位，堵着路
            {
                return 999;
            }
            else if (startUnit.isInfantry()
            && (terrainType == GridType.Sea || terrainType == GridType.Reef))
            {
                return 999;
            }
            else if (startUnit.isPlane())
            {
                return 2;
            }
            else if (startUnit.isShip()
                && terrainType != GridType.Sea)
            {
                return 999;
            }
            else if (startUnit.isVehicle()
                && (terrainType == GridType.Mountain || terrainType == GridType.Sea || terrainType == GridType.Reef))
            {
                return 999;
            }
            else
            {
                return oilCost;
            }
        }
    }
    public int oilCost;

    //移动到这里时还剩下的油量
    public int oilLeft;

    //父格子
    public Node parent;

    public Node(Point pos, int oilCost, GridType type)
    {
        this.pos = pos;
        this.oilCost = oilCost;
        terrainType = type;
    }
}