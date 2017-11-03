using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//Author: MaxLykoS
//UpdateTime: 2017/10/28

public class CombatController : MonoBehaviour {
    public static int p1CashTotal = 3000;
    public static int p2CashTotal = 3000;

    public static int p1CitysCount = 0;
    public static int p2CitysCount = 0;

    public static bool isPlayer1=false;  //玩家作为玩家1进行游戏
    public static bool isPlayer2=false;  //玩家作为玩家2进行游戏

    public bool P1Ready
    {
        get { return p1Ready; }
    }
    private bool p1Ready = false;
    public bool P2Ready
    {
        get { return p2Ready; }
    }
    private bool p2Ready = false;

    public int Rounds = 1;   //当前是第几回合

    private int firstClick = 1;

    void Awake()
    {
        isPlayer1 = true;
        isPlayer2 = true;
    }
    void Start ()
    {
        FindUI();

        GridContainer.isEditorMode = false;   //开启战斗模式

        MapLoader.LoadCustomLevel("Test");
        GridContainer.GameStartKey = true;
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

    /// <summary>
    /// 战斗模式下点击格子后的回调函数
    /// </summary>
    /// <param name="clickPos"></param>
    public void ClickChooseGridEventHandler(Point clickPos)
    {

        #region 显示点击格子的详细资料
        ShowGridDetailPanel(clickPos);
        #endregion

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
                PathNav.CurrentMovingUnit.UnitMoveToTargetPos(clickPos, OnMoveEnd);
                return;
            }
        }
        #endregion

        #region 第三次点击，即攻击
        if (firstClick == 3)
        {
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

                        b.BeCapture(cu);

                        PathNav.CurrentMovingUnit.SetMovedToken();//攻击完了就标记已运动

                        firstClick = 1;
                        PathNav.CurrentMovingUnit.StopShowAttackRange();
                    }
                }
            }
            else if (PathNav.CurrentMovingUnit.CheckAttackable(clickPos))  //能攻击到该位置
            {
                Unit targetUnit;
                if (GridContainer.Instance.UnitDic.TryGetValue(clickPos, out targetUnit)
                    && targetUnit.Side != PathNav.CurrentMovingUnit.Side)//该位置有个敌人单位
                {
                    #region 攻击和被攻击
                    PathNav.CurrentMovingUnit.AttackInitiative(targetUnit);  //攻击

                    if (PathNav.CurrentMovingUnit.isAlive())  //如果自己还或者
                    {
                        if (targetUnit.isAlive())  //如果敌人还活着，就会被动攻击我
                        {
                            targetUnit.AttackPassive(PathNav.CurrentMovingUnit);
                        }
                        else
                        {
                            targetUnit.BeDestroyed();   //如果敌人挂了，就被摧毁
                        }
                    }

                    if (!PathNav.CurrentMovingUnit.isAlive())  //自己被敌人被动攻击干掉后，就被摧毁
                    {
                        PathNav.CurrentMovingUnit.BeDestroyed();
                    }
                    #endregion

                    PathNav.CurrentMovingUnit.SetMovedToken();//攻击完了就标记已运动

                    firstClick = 1;
                    PathNav.CurrentMovingUnit.StopShowAttackRange();
                }
            }
            else if (PathNav.CurrentMovingUnit.Side ==
                GridContainer.Instance.UnitDic[clickPos].Side
                || GridContainer.Instance.UnitDic[clickPos] is ITransport)       //上车，运输
            {
                Debug.Log("asd");
            }
        }
        #endregion
    }
    void OnMoveEnd()
    {
        firstClick = 3;
        if (PathNav.bMoving)
        {
            if (!PathNav.CurrentMovingUnit.ShowAttackRange())   
            {
                firstClick = 1;    //如果该单位没有攻击能力，直接跳过！转入第一次点击状态
                PathNav.CurrentMovingUnit.SetMovedToken();
            }
        }
        else
        {
            firstClick = 1;
        }
    }
    /// <summary>
    /// 战斗模式下取消点击的回调函数
    /// </summary>
    public void CancelChooseGridEventHandler()
    {
        #region 当点击进入攻击态，且玩家不打算攻击时，设置单位的不可移动标志
        if (PathNav.CurrentMovingUnit != null)
        {
            if (firstClick == 3)
            {
                PathNav.CurrentMovingUnit.SetMovedToken();
            }
        }
        #endregion

        PathNav.StopShowUnitMoveRange();
        PathNav.CurrentMovingUnit.StopShowAttackRange();
        HideBuildingPanel();
        firstClick = 1;
    }

    /// <summary>
    /// 显示当前所点击格子的详细资料
    /// </summary>
    /// <param name="clickPos"></param>
    void ShowGridDetailPanel(Point clickPos)
    {

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
            GridContainer.Instance.AddUnit(CameraController.CurrentClickPos, newType, side);

            Unit u = GridContainer.Instance.UnitDic[CameraController.CurrentClickPos];
            u.StopMoveable();
            u.SetMovedToken();

            UpdateTotalCashText();
            HideBuildingPanel();
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
                p2CashTotal -= price;  //扣钱
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
                p1CashTotal -= price;  //扣钱
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
                        p1Ready = true;
                        textP1Ready.text = "Player 1 is ready!";
                    }
                        break;
                }
            case 2:
                {
                    if (isPlayer2)
                    {
                        p2Ready = true;
                        textP2Ready.text = "Player 2 is ready!";
                    }
                    break;
                }
        }

        if (p1Ready && p2Ready)
        {
            PathNav.bMoving = false;   //让正在移动的棋子立刻到达终点，进入下一回合
            NextRound();
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

        foreach (Unit u in GridContainer.Instance.UnitDic.Values)
        {
            u.SetMoveableDestroyToken();
        }
        p1Ready = false;
        p2Ready = false;



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
}
