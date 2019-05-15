using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/11/6

public class TransportChopter : Plane,ITransport {

    public static int Price = 8000;

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/TransportChopterEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/TransportChopterFriendly");
        base.SetFriendly();
    }

    protected override List<Point> SetAttackRange()
    {
        List<Point> p = new List<Point>();
        return p;
    }


    public Unit PayLoad
    {
        get;
        set;
    }

    public bool Load(Unit u)
    {
        if (u is Men && PayLoad == null)
        {
            PayLoad = u;

            u.StopHighLight();
            u.StopShowAttackRange();

            PayLoad.Hide();

            return true;
        }
        return false;
    }

    public bool UnLoad(Point pos)
    {
        if (!this.CheckCouldMoveTo(GridContainer.Instance.TerrainDic[pos]))
            return false;

        PayLoad.Show(pos);
        PayLoad.StopMoveable();
        PayLoad.SetMovedToken();
        NetMgr.srvConn.gameNet.SendMoveDone(PayLoad.gridID);
        PayLoad = null;

        StopMoveable();
        SetMovedToken();
        NetMgr.srvConn.gameNet.SendMoveDone(gridID);

        return true;
    }
}
