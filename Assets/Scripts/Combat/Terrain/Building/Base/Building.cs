using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building : TerrainBase
{
    protected int oldCapturePoint;

    public int capturePoint;

    public Unit CurrentCapturingUnit
    {
        get { return currentCapturingUnit; }
    }
    private Unit currentCapturingUnit;

	void Start ()
    {
        Debug.Log(oldCapturePoint);
	}

    public void BeCapture(Men captureUnit)
    {
        if (currentCapturingUnit != captureUnit)   //如果来占领的人换了，就得重新占领
        {
            currentCapturingUnit = captureUnit;
            capturePoint = oldCapturePoint;
        }

        currentCapturingUnit = captureUnit;

        capturePoint -= captureUnit.CaptureCapablility;

        if (capturePoint <= 0)  //被占领了
        {
            GameStatNotifier.Instance.Capture[(int)captureUnit.Side]++;

            #region 判断输赢
            if (gridType == GridType.HQ)
            {
                bool isPlayer1 = Global.Instance.gameInfo.IsHost;
                CombatController cc = GameObject.Find("CombatController").GetComponent<CombatController>();
                if (isPlayer1)
                {
                    if (Side == SideType.Friendly)
                    {
                        Debug.Log("输了");
                        NetMgr.srvConn.gameNet.SendStatus(false, GameNet.GameStatus.Lose);
                        cc.GameEndHandler(false);
                    }
                    else if (Side == SideType.Enemy)
                    {
                        Debug.Log("赢了");
                        NetMgr.srvConn.gameNet.SendStatus(true, GameNet.GameStatus.Win);
                        cc.GameEndHandler(true);
                    }
                }
                else
                {
                    if (Side == SideType.Friendly)
                    {
                        Debug.Log("赢了");
                        NetMgr.srvConn.gameNet.SendStatus(false, GameNet.GameStatus.Win);
                        cc.GameEndHandler(true);
                    }
                    else if (Side == SideType.Enemy)
                    {
                        Debug.Log("输了");
                        NetMgr.srvConn.gameNet.SendStatus(false, GameNet.GameStatus.Lose);
                        cc.GameEndHandler(false);
                    }
                }
            }
            #endregion
            #region 判断城市增减
            if (gridType == GridType.City)
            {
                bool isPlayer1Capture = captureUnit.Side==SideType.Friendly?true:false;
                if (Side == SideType.Neutral)
                {
                    if(isPlayer1Capture)
                        CombatController.p1CitysCount++;
                    else
                        CombatController.p2CitysCount++;
                }
                else if (Side == SideType.Friendly)
                {
                    if (!isPlayer1Capture)
                    {
                        CombatController.p1CitysCount--;
                        CombatController.p2CitysCount++;
                    }
                }
                else if(Side==SideType.Enemy)
                {
                    if (isPlayer1Capture)
                    {
                        CombatController.p1CitysCount++;
                        CombatController.p1CitysCount--;
                    }
                }
            }
            #endregion
            capturePoint = oldCapturePoint;
            switch (captureUnit.Side)
            {
                case SideType.Enemy:
                    {
                        SetEnemy();
                    }
                    break;
                case SideType.Friendly:
                    {
                        SetFriendly();
                    }
                    break;
            }
        }
    }
    public void ResetCapturePoint()
    {
        capturePoint = oldCapturePoint;
    }

    public override void OnInstatiate()
    {
        oldCapturePoint = capturePoint;    //记录原有的占领点数
    }
}
