using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//Author: MaxLykoS
//UpdateTime: 2017/11/6

public class CombatController : MonoBehaviour {
    public static int p1CashTotal = 3000;
    public static int p2CashTotal = 3000;

    public static int p1CitysCount = 0;
    public static int p2CitysCount = 0;

    public static bool isPlayer1=false;  //玩家作为玩家1进行游戏
    public static bool isPlayer2=false;  //玩家作为玩家2进行游戏

    private bool p1Ready = false;
    private bool p2Ready = false;

    private int Rounds = 1;   //当前是第几回合

    private int firstClick = 1;

    void Awake()
    {
        if (Global.Instance.gameInfo.IsHost)
        {
            isPlayer1 = true;
            isPlayer2 = false;
        }
        else
        {
            isPlayer2 = true;
            isPlayer1 = false;
        }
    }
    void Start ()
    {
        Init();

        FindUI();

        GridContainer.isEditorMode = false;   //开启战斗模式

        MapLoader.LoadCustomLevel(Global.Instance.gameInfo.MapName);
        /*MapLoader.LoadCustomLevel("CustomLevel1");
        string host = "127.0.0.1";
        int port = 1234;
        NetMgr.srvConn.proto = new ProtocolBytes();  //用来接受服务器发送的信息
        NetMgr.srvConn.Connect(host, port);
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString("max");
        protocol.AddString("1234");
        NetMgr.srvConn.Send(protocol);
        isPlayer1 = true;
        isPlayer2 = true;
        Global.Instance.gameInfo.playerInfo.PlayerName = "max";*/

        GameStatNotifier.Instance.Init();
        GridContainer.GameStartKey = true;
        GameStatNotifier.Instance.StartTimer();
    }

    void FindUI()
    {
        RoundsLabel = GameObject.Find("Canvas/RoundsLabel");

        RoundNumText = GameObject.Find("Canvas/RoundsLabel/Text").GetComponent<Text>();
        textP1Ready = GameObject.Find("Canvas/Player1/BtnP1Ready/Text").GetComponent<Text>();
        textP2Ready = GameObject.Find("Canvas/Player2/BtnP2Ready/Text").GetComponent<Text>();

        textP1CashTotal = GameObject.Find("Canvas/Player1/CashTotalText").GetComponent<Text>();
        textP2CashTotal = GameObject.Find("Canvas/Player2/CashTotalText").GetComponent<Text>();
    }
    private void Init()
    {
        PayloadLandRange = new List<TerrainBase>();

        p1CashTotal = 3000;
        p2CashTotal = 3000;

        AddListener();
    }

    /// <summary>
    /// 战斗模式下点击格子后的回调函数
    /// </summary>
    /// <param name="clickPos"></param>
    public void ClickChooseGridEventHandler(Point clickPos)
    {
        if (isPlayer1)
        {
            if (p1Ready)
                return;
        }
        else if(isPlayer2)
        {
            if (p1Ready==false)
                return;
        }

        couldCancel = true;

        #region 第一次点击   //选中单位，选中建筑
        if (firstClick==1)
        {
            TerrainBase tb = GridContainer.Instance.TerrainDic[clickPos];
            Unit u;
            if (GridContainer.Instance.UnitDic.TryGetValue(clickPos,out u))   //玩家点击的是单位,此外，这个格子不能已经走过了
            {
                HideBuildingPanel();
                #region 区分玩家点击
                if (isPlayer1 && u.Side == SideType.Friendly)  //玩家是玩家1
                {
                    if (u.IsMoveable)
                    {
                        u.SetHighLight();
                        u.ShowMoveRange();
                        firstClick=2;
                        return;
                    }
                }

                if (isPlayer2 && u.Side == SideType.Enemy)  //玩家是玩家2
                {
                    if (u.IsMoveable)
                    {
                        u.SetHighLight();
                        u.ShowMoveRange();
                        firstClick=2;
                        return;
                    }
                }
                #endregion
            }
            else if (tb is Building
                &&tb.gridType!=GridType.HQ
                &&tb.gridType!=GridType.City)             //玩家点击的是建筑且不能是HQ也不能是City
            {
                #region 区分玩家点击
                if (isPlayer1 && tb.Side == SideType.Friendly)  //玩家1点击玩家1的建筑
                {
                    HideBuildingPanel();
                    ShowBuildingPanel(clickPos);
                    return;
                }

                if (isPlayer2 && tb.Side == SideType.Enemy)  //玩家2点击玩家2的建筑
                {
                    HideBuildingPanel();
                    ShowBuildingPanel(clickPos);
                    return;
                }
                #endregion
            }
            else if(tb is Landform)   //玩家点击的是地形
            {
                
            }

            return;
        }

        #endregion

        #region 第二次点击   //单位移动
        if(firstClick==2)
        {
            if (PathNav.ReachablePoints.Contains(clickPos))  //如果格子能走到那里
            {
                firstClick=0;   //单位移动时屏蔽点击
                PathNav.bMoving = true;
                PathNav.StopShowUnitMoveRange();

                NetMgr.srvConn.gameNet.SendMove(PathNav.CurrentMovingUnit.gridID,clickPos);  //同步移动位置

                //PathNav.CurrentMovingUnit.UnitMoveToTargetPos(clickPos, OnMoveEnd);
                return;
            }
        }
        #endregion

        #region 第三次点击，即攻击,或上车下车
        if (firstClick == 3)
        {
            #region 占领
            if (clickPos == PathNav.CurrentMovingUnit.gridID)   //玩家点击的是脚底下
            {
                TerrainBase tb = GridContainer.Instance.TerrainDic[clickPos]; 
                if (tb.Side != PathNav.CurrentMovingUnit.Side)   //建筑跟自己是不同阵营的，才可以占领
                {
                    if (tb is Building&&PathNav.CurrentMovingUnit is Men)//点击的是建筑的子类，即可以占领,同时当前点击的单位可以占领
                    {
                        Building b = (Building)tb;
                        Men cu = (Men)PathNav.CurrentMovingUnit;

                        Debug.Log("尝试占领");
                        NetMgr.srvConn.gameNet.SendBuildingCapture(b.gridID);

                        PathNav.CurrentMovingUnit.SetMovedToken();//攻击完了就标记已运动
                        NetMgr.srvConn.gameNet.SendMoveDone(PathNav.CurrentMovingUnit.gridID);

                        firstClick = 1;
                        PathNav.CurrentMovingUnit.StopShowAttackRange();
                    }
                }
            }
            #endregion
            else if (PathNav.CurrentMovingUnit.CheckAttackable(clickPos))  //能攻击到该位置
            {
                Unit targetUnit;
                #region 攻击
                if (GridContainer.Instance.UnitDic.TryGetValue(clickPos, out targetUnit)
                    && targetUnit.Side != PathNav.CurrentMovingUnit.Side)//该位置有个敌人单位
                {
                    #region 攻击和被攻击
                    NetMgr.srvConn.gameNet.SendAttackInitiative(PathNav.CurrentMovingUnit.gridID, targetUnit.gridID);

                    #endregion

                    PathNav.CurrentMovingUnit.SetMovedToken();//攻击完了就标记已运动
                    NetMgr.srvConn.gameNet.SendMoveDone(PathNav.CurrentMovingUnit.gridID);

                    firstClick = 1;
                    PathNav.CurrentMovingUnit.StopShowAttackRange();

                    if (!PathNav.CurrentMovingUnit.isAlive())  //自己被敌人被动攻击干掉后，就被摧毁
                    {
                        NetMgr.srvConn.gameNet.SendUnitDestroy(PathNav.CurrentMovingUnit.gridID);
                    }

                    return;
                }
                #endregion
                #region 上车
                else if (GridContainer.Instance.UnitDic.TryGetValue(clickPos, out targetUnit)
                    && PathNav.CurrentMovingUnit.Side ==
                GridContainer.Instance.UnitDic[clickPos].Side
                && GridContainer.Instance.UnitDic[clickPos] is ITransport)    //上车
                {
                    NetMgr.srvConn.gameNet.SendLoadUnit(PathNav.CurrentMovingUnit.gridID, targetUnit.gridID);
                }
                #endregion
            }
            #region 下车
            else if (PathNav.CurrentMovingUnit is ITransport)   //卸载
            {
                
                if (PathNav.CurrentMovingUnit.gridID - clickPos == 1) //距离相差为1
                {
                    //Check Landable
                    ITransport it = (ITransport)PathNav.CurrentMovingUnit;
                    if (it.PayLoad.CheckCouldMoveTo(GridContainer.Instance.TerrainDic[clickPos]))//如果该地点可以下车
                    {
                        NetMgr.srvConn.gameNet.SendUnloadUnit(clickPos, PathNav.CurrentMovingUnit.gridID);
                    }
                }
            }
            #endregion
        }
        #endregion
    }
    /// <summary>
    /// 货物卸载范围
    /// </summary>
    private List<TerrainBase> PayloadLandRange;
    void OnMoveEnd()
    {
        firstClick = 3;
        if (PathNav.bMoving)
        {
            if (!PathNav.CurrentMovingUnit.ShowAttackRange())   
            {
                if (PathNav.CurrentMovingUnit is ITransport)   //正在走的这个单位实现了运输接口
                {
                    ITransport it = (ITransport)PathNav.CurrentMovingUnit;
                    if (it.PayLoad != null)
                    {
                        #region 求下车范围
                        Point[] p4 = { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
                        TerrainBase tb;
                        Point id = PathNav.CurrentMovingUnit.gridID;
                        foreach (Point p in p4)
                        {
                            if (GridContainer.Instance.TerrainDic.TryGetValue(new Point(p.X + id.X, p.Z + id.Z), out tb))  //得到卸货范围
                            {
                                if (it.PayLoad.CheckCouldMoveTo(tb))
                                {
                                    PayloadLandRange.Add(tb);
                                    tb.SetHighLight();
                                }
                            }
                        }
                        #endregion

                        firstClick = 3;   //这个单位里装着payload，进入第三次点击状态，准备卸载
                        return;
                    }
                }

                PathNav.CurrentMovingUnit.SetMovedToken();
                NetMgr.srvConn.gameNet.SendMoveDone(PathNav.CurrentMovingUnit.gridID);

                firstClick = 1;    //如果该单位没有攻击能力，直接跳过！转入第一次点击状态
            }
        }
        else
        {
            firstClick = 1;
        }
    }

    #region 比赛结束
    public void GameEndHandler(bool win)
    {
        PanelMgr.Instance.OpenPanel<UI.Tip.WarningTip>("",win?"You win!":"You lose!");
        NetMgr.srvConn.msgDist.ClearEventDic();
        Invoke("MoveToStatPanel", 5f);
    }
    void MoveToStatPanel()
    {
        GridContainer.Instance.TerrainLayer.gameObject.SetActive(false);
        GridContainer.Instance.UnitLayer.gameObject.SetActive(false);

        PanelMgr.Instance.ClosePanel("UI.Tip.WarningTip");
        PanelMgr.Instance.ClosePanel("UI.Panel.GridDetailPanel");
        PanelMgr.Instance.OpenPanel<UI.Panel.GameStatPanel>("");
    }
    #endregion

    private bool couldCancel = false;
    /// <summary>
    /// 战斗模式下取消点击的回调函数
    /// </summary>
    public void CancelChooseGridEventHandler()
    {
        if (couldCancel)
        {
            if (firstClick == 0) return;   //屏蔽一切操作

            #region 当点击进入攻击态，且玩家不打算攻击时，设置单位的不可移动标志
            if (PathNav.CurrentMovingUnit != null)
            {
                if (firstClick == 3)
                {
                    PathNav.CurrentMovingUnit.SetMovedToken();
                    NetMgr.srvConn.gameNet.SendMoveDone(PathNav.CurrentMovingUnit.gridID);
                }
            }
            #endregion

            PathNav.StopShowUnitMoveRange();
            if (PayloadLandRange.Count != 0)   //卸货范围修改
            {
                foreach (TerrainBase tb in PayloadLandRange)
                    tb.StopHighLight();
                PayloadLandRange.Clear();
            }

            if (PathNav.CurrentMovingUnit != null)
                PathNav.CurrentMovingUnit.StopShowAttackRange();
            HideBuildingPanel();
            firstClick = 1;

            couldCancel = false;
        }
    }

    /// <summary>
    /// 显示当前所点击格子的详细资料
    /// </summary>
    /// <param name="clickPos"></param>
    public void ShowGridDetailPanel(Point pointPos)
    {
        PanelMgr.Instance.ClosePanel("UI.Panel.GridDetailPanel");
        Unit u;
        TerrainBase tb;
        if (GridContainer.Instance.UnitDic.TryGetValue(pointPos, out u))
        {
            //部队  图片 名字 血量 伤害 护甲 伤害类型
            Texture m = u.gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture;
            string s1, s2, s3, s4, s5;
            s1 = "Name:"+u.gridType.ToString();
            s2 = "HP:"+u.HP.ToString();
            s3 = "Dmg:"+(u.FirePower * (u.HP / 100)).ToString();
            s4 = "Armor:"+u.armorType.ToString();
            s5 = "Type:"+u.attackType.ToString();
            PanelMgr.Instance.OpenPanel<UI.Panel.GridDetailPanel>("",m,s1,s2,s3,s4,s5);
        }
        else if (GridContainer.Instance.TerrainDic.TryGetValue(pointPos, out tb))
        {
            //地形  图片 名字 防御力 油量消耗      
            Texture m = tb.gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture;
            string s1, s2, s3;
            s1 = "Name:" + tb.gridType.ToString();
            s2 = "DEF:" + tb.DefendStar;
            s3 = "OilCost:" + tb.OilCost;
            PanelMgr.Instance.OpenPanel<UI.Panel.GridDetailPanel>("",m,s1,s2,s3);
        }

        //PanelMgr.Instance.ClosePanel("UI.Panel.LobbyPanel + AchieveTip")
    }

    #region 建造面板
    public Transform FactoryPanel;
    public Transform AirportPanel;
    public Transform ShipyardPanel;
    /// <summary>
    /// 显示建造面板
    /// </summary>
    void ShowBuildingPanel(Point clickPos)
    {
        TerrainBase tb = GridContainer.Instance.TerrainDic[clickPos];
        switch (tb.gridType)
        {
            case GridType.Factory: 
                FactoryPanel.gameObject.SetActive(true);
                break;
            case GridType.Airport:
                AirportPanel.gameObject.SetActive(true);
                break;
            case GridType.Shipyard:
                ShipyardPanel.gameObject.SetActive(true);
                break;
            default: throw new Exception("无法显示该建筑建造面板！");
        }
    }
    /// <summary>
    /// 隐藏建造面板
    /// </summary>
    void HideBuildingPanel()
    {
        FactoryPanel.gameObject.SetActive(false);
        AirportPanel.gameObject.SetActive(false);
        ShipyardPanel.gameObject.SetActive(false);
    }
    /// <summary>
    /// 建造某单位按钮
    /// </summary>
    /// <param name="type"></param>
    public void BtnBuildUnit(int type)
    {
        SideType side = GridContainer.Instance.TerrainDic[CameraController.CurrentClickPos].Side;

        GridType newType = (GridType)type;

        int price = GetUnitPrice(newType);

        if (CheckIfAfford(price, side))
        {
            NetMgr.srvConn.gameNet.SendCreateUnit(type, CameraController.CurrentClickPos);
        }
    }
    /// <summary>
    /// 得到当前单位价格
    /// </summary>
    /// <param name="newType"></param>
    /// <returns></returns>
    int GetUnitPrice(GridType newType)
    {
        switch (newType)
        {
            case GridType.Infantry:
                return Infantry.Price;
            case GridType.ATInfantry:
                return ATInfantry.Price;
            case GridType.Scout:
                return Scout.Price;
            case GridType.Transporter:
                return Transporter.Price;
            case GridType.LightTank:
                return LightTank.Price;
            case GridType.Artillery:
                return Artillery.Price;
            case GridType.ATAir:
                return ATAir.Price;
            case GridType.HeavyTank:
                return HeavyTank.Price;
            case GridType.CannonTank:
                return CannonTank.Price;
            case GridType.Rockets:
                return Rockets.Price;
            case GridType.ATAirMissile:
                return ATAirMissile.Price;
            case GridType.Fighter:
                return Fighter.Price;
            case GridType.Bomber:
                return Bomber.Price;
            case GridType.Chopter:
                return Chopter.Price;
            case GridType.TransportChopter:
                return TransportChopter.Price;
            case GridType.BattleShip:
                return BattleShip.Price;
            case GridType.Destroyer:
                return Destroyer.Price;
            case GridType.LandingShip:
                return LandingShip.Price;
            case GridType.Submarine:
                return Submarine.Price;
            default: throw new Exception("购买未知单位");
        }
    }
    /// <summary>
    /// 检查能否买得起
    /// </summary>
    /// <param name="price"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    bool CheckIfAfford(int price,SideType side)
    {
        if (side == SideType.Enemy)   //点击的是p2
        {
            if (p2CashTotal < price)
            {
                //p2钱不够
                return false;
            }
            else
            {
                //p2CashTotal -= price;  //扣钱
                //GameStatNotifier.Instance.Cost[1] += price;
                return true;
            }
        }
        else                          //点击的是p1
        {
            if (p1CashTotal < price)
            {
                //p1钱不够
                return false;
            }
            else
            {
                //p1CashTotal -= price;  //扣钱
                //GameStatNotifier.Instance.Cost[0] += price;
                return true;
            }
        }
    }
    #endregion

    #region 进入下一回合
    private GameObject RoundsLabel;
    private Text textP1CashTotal, textP2CashTotal;
    private Text RoundNumText;
    private Text textP1Ready, textP2Ready;
    private delegate void OnFinishAnim();
    /// <summary>
    /// 都准备好了就到到下一局
    /// </summary>
    /// <param name="order"></param>
    public void BtnPlayerReady(int order)
    {
        switch (order)
        {
            case 1:
                {
                    if (isPlayer1)
                    { 
                        NetMgr.srvConn.gameNet.SendStatus(true, GameNet.GameStatus.Ready);
                    }
                    break;
                }
            case 2:
                {
                    if (isPlayer2)
                    {
                        NetMgr.srvConn.gameNet.SendStatus(false, GameNet.GameStatus.Ready);
                    }
                    break;
                }
        }
    }
    /// <summary>
    /// 进入下一回合
    /// </summary>
    void AnimAndNextRound()
    {
        #region 进入下一回合动画
        StartCoroutine(INextRoundAnim(NextRound));
        #endregion
    }
    void NextRound()
    {
        #region 场景各种状态归零
        if (PathNav.CurrentMovingUnit != null)
        {
            PathNav.CurrentMovingUnit.StopShowAttackRange();
        }
        firstClick = 1;
        #endregion


        p1CashTotal += 1000 * p1CitysCount;
        p2CashTotal += 1000 * p2CitysCount;

        GameStatNotifier.Instance.Earn[0] += 1000 * p1CitysCount;
        GameStatNotifier.Instance.Earn[1] += 1000 * p2CitysCount;

        foreach (Unit u in GridContainer.Instance.UnitDic.Values)
        {
            u.SetMoveableDestroyToken();
        }
        p1Ready = false;
        p2Ready = false;


        GameStatNotifier.Instance.Round++;
        Rounds++;
        RoundNumText.text = "Round " + Rounds.ToString();
        textP1Ready.text = "Player 1 not ready";
        textP2Ready.text = "Player 2 not ready";
        Debug.Log("进入到下一回合");

        UpdateTotalCashText();
    }
    void UpdateTotalCashText()
    {
        textP1CashTotal.text = "Cash " + p1CashTotal.ToString();
        textP2CashTotal.text = "Cash " + p2CashTotal.ToString();
    }
    IEnumerator INextRoundAnim(OnFinishAnim NextRound)
    {
        Debug.Log("下一回合动画");
        NextRound();
        yield return 0;
    }
    #endregion

    void AddListener()
    {
        NetMgr.srvConn.msgDist.AddListener("UnitMove", RecvUnitMove);
        NetMgr.srvConn.msgDist.AddListener("GameStatus", RecvGameStatus);
        NetMgr.srvConn.msgDist.AddListener("MoveDone", RecvMoveDone);
        NetMgr.srvConn.msgDist.AddListener("CreateUnit", RecvCreateUnit);
        NetMgr.srvConn.msgDist.AddListener("AttackInitiative", RecvAttackInitiative);
        NetMgr.srvConn.msgDist.AddListener("AttackPassive", RecvAttackPassive);
        NetMgr.srvConn.msgDist.AddListener("UnitDestroy", RecvUnitDestroy);
        NetMgr.srvConn.msgDist.AddListener("BuildingCapture", RecvBuildingCapture);
        NetMgr.srvConn.msgDist.AddListener("LoadUnit", RecvLoadUnit);
        NetMgr.srvConn.msgDist.AddListener("UnloadUnit", RecvUnloadUnit);
    }
    #region 监听事件
    //单位移动
    //UnitMove 单位坐标 目的地坐标
    private void RecvUnitMove(ProtocolBase protocol)
    {
        ProtocolBytes info = (ProtocolBytes)protocol;
        int start=0;
        info.GetString(start, ref start);  //"UnitMove"
        Point startPo = info.GetPoint(start, ref start);
        Point endPo = info.GetPoint(start, ref start);

        PathNav.bMoving = true;

        GridContainer.Instance.UnitDic[startPo].UnitMoveToTargetPos(endPo,OnMoveEnd);
    }

    //游戏状态
    //GameStatus 玩家号0/1 状态
    private void RecvGameStatus(ProtocolBase protocol)
    {
        int start = 0;
        ProtocolBytes info = (ProtocolBytes)protocol;
        info.GetString(start, ref start); //GameStatus
        int id = info.GetInt(start, ref start);
        if (id == 0)
        {
            p1Ready = true;
            textP1Ready.text = "Player 1 is ready!";
        }
        else
        {
            p2Ready = true;
            textP2Ready.text = "Player 2 is ready!";
        }

        if (p1Ready && p2Ready)
        {
            PathNav.bMoving = false;   //让正在移动的棋子立刻到达终点，进入下一回合
            NextRound();
        }

        int status = info.GetInt(start, ref start);

        int _id = Global.Instance.gameInfo.IsHost ? 0 : 1;
        if (id == _id)
        {
            if (status == 1)
            {
                //NetMgr.srvConn.gameNet.SendStatus(true, GameNet.GameStatus.Win);
                GameEndHandler(true);
            }
            else if (status == 2)
            {
                //NetMgr.srvConn.gameNet.SendStatus(false, GameNet.GameStatus.Lose);
                GameEndHandler(false);
            }
        }
        else
        {
            if (status == 1)
            {
                NetMgr.srvConn.gameNet.SendStatus(false, GameNet.GameStatus.Lose);
                GameEndHandler(false);
            }
            else if (status == 2)
            {
                NetMgr.srvConn.gameNet.SendStatus(true, GameNet.GameStatus.Win);
                GameEndHandler(true);
            }
        }
    }

    //移动结束
    //MoveDone 坐标
    private void RecvMoveDone(ProtocolBase protocol)
    {
        int start = 0;
        ProtocolBytes info = (ProtocolBytes)protocol;
        info.GetString(start,ref start);  //MoveDone
        Point p = info.GetPoint(start, ref start);
        Unit u = GridContainer.Instance.UnitDic[p];
        u.SetMovedToken();
        u.StopShowAttackRange();
        firstClick = 1;
    }

    //创建部队
    //CreateUnit 部队类型 坐标
    private void RecvCreateUnit(ProtocolBase protocol)
    {
        int start = 0;
        ProtocolBytes info = (ProtocolBytes)protocol;
        info.GetString(start, ref start);
        int type = info.GetInt(start, ref start);
        Point p = info.GetPoint(start, ref start);

        SideType side = GridContainer.Instance.TerrainDic[p].Side;
        GridType newType = (GridType)type;
        GridContainer.Instance.AddUnit(p, newType, side);
        GameStatNotifier.Instance.Create[(int)side]++;
        int cashCost = (int)Type.GetType(newType.ToString()).GetField("Price").GetValue(null);
        GameStatNotifier.Instance.Cost[side == SideType.Friendly ? 0 : 1] += cashCost;
        if (side == SideType.Friendly)
            p1CashTotal -= cashCost;
        else if (side == SideType.Enemy)
            p2CashTotal -= cashCost;

        Unit u = GridContainer.Instance.UnitDic[p];
        u.StopMoveable();
        u.SetMovedToken();
        NetMgr.srvConn.gameNet.SendMoveDone(u.gridID);

        UpdateTotalCashText();
        HideBuildingPanel();
    }

    //部队主动攻击
    //AttackInitiative 攻击者坐标 被攻击坐标
    private void RecvAttackInitiative(ProtocolBase protocol)
    {
        int start = 0;
        ProtocolBytes pb = (ProtocolBytes)protocol;
        pb.GetString(start, ref start);            //AttackInitiative
        Point s = pb.GetPoint(start, ref start);
        Point e = pb.GetPoint(start, ref start);

        Unit a = GridContainer.Instance.UnitDic[s];
        Unit b = GridContainer.Instance.UnitDic[e]; 
        a.AttackInitiative(b);  //攻击

        if (a.isAlive())  //如果自己还活着
        {
            if (b.isAlive())  //如果敌人还活着，就会被动攻击我，长程火力除外
            {
                if (!a.isLongRangeUnit())
                {
                    NetMgr.srvConn.gameNet.SendAttackPassive(b.gridID, a.gridID);
                }
            }
            else
            {
                NetMgr.srvConn.gameNet.SendUnitDestroy(b.gridID);
                //b.BeDestroyed();   //如果敌人挂了，就被摧毁
            }
        }
    }

    //部队被动攻击
    //AttackPassive 攻击者坐标 被攻击坐标
    private void RecvAttackPassive(ProtocolBase protocol)
    {
        int start = 0;
        ProtocolBytes pb = (ProtocolBytes)protocol;
        pb.GetString(start, ref start);            //AttackPassive
        Point s = pb.GetPoint(start, ref start);
        Point e = pb.GetPoint(start, ref start);

        Unit a = GridContainer.Instance.UnitDic[s];
        Unit b = GridContainer.Instance.UnitDic[e];

        a.AttackPassive(b);
        firstClick = 1;
    }

    //销毁单位
    //UnitDestroy 被销毁单位坐标
    private void RecvUnitDestroy(ProtocolBase protocol)
    {
        int start = 0;
        ProtocolBytes pb = (ProtocolBytes)protocol;
        pb.GetString(start, ref start); //UnitDestroy
        Point p = pb.GetPoint(start, ref start);

        Unit u;
        if(GridContainer.Instance.UnitDic.TryGetValue(p, out u))
            u.BeDestroyed();
    }

    //占领建筑
    //BuildingCapture 坐标
    private void RecvBuildingCapture(ProtocolBase protocol)
    {
        int start = 0;
        ProtocolBytes pb = (ProtocolBytes)protocol;
        pb.GetString(start, ref start);  //UnitDestroy
        Point p = pb.GetPoint(start, ref start);
        Building b = GridContainer.Instance.TerrainDic[p] as Building;
        Men m = GridContainer.Instance.UnitDic[p] as Men;
        b.BeCapture(m);
    }

    //装载单位
    //LoadUnit 单位 载具
    private void RecvLoadUnit(ProtocolBase protocol)
    {
        int start = 0;
        ProtocolBytes pb = (ProtocolBytes)protocol;
        pb.GetString(start, ref start);//LoadUnit
        Point unit = pb.GetPoint(start, ref start);
        Point loader = pb.GetPoint(start, ref start);

        ITransport carrier = (ITransport)GridContainer.Instance.UnitDic[loader];

        if (carrier.Load(GridContainer.Instance.UnitDic[unit]))
        {
            firstClick = 1;
        }
    }

    //卸载单位
    //UnloadUnit 单位 载具
    private void RecvUnloadUnit(ProtocolBase protocol)
    {
        int start = 0;
        ProtocolBytes pb = (ProtocolBytes)protocol;
        pb.GetString(start, ref start);//LoadUnit
        Point unit = pb.GetPoint(start, ref start);
        Point loader = pb.GetPoint(start, ref start);

        ITransport it = (ITransport)GridContainer.Instance.UnitDic[loader];
        it.UnLoad(unit);
        foreach (TerrainBase tb in PayloadLandRange)
            tb.StopHighLight();
        PayloadLandRange.Clear();

        firstClick = 1;
    }
    #endregion

    void Update()
    {
        NetMgr.Update();//消息监听

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PanelMgr.Instance.OpenPanel<UI.Panel.PausePanel>("");
        }
    }
}