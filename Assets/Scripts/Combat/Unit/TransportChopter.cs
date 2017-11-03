using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

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

    public void Load(Unit u, Point pos)
    {
        if (u is Men)
        {
            PayLoad = u;
            GridContainer.Instance.UnitDic.Remove(pos);
            u.gridID = null;
            Destroy(u.gameObject);
        }
    }

    public bool UnLoad(Point pos)
    {
        if (!this.CheckCouldMoveTo(GridContainer.Instance.TerrainDic[pos]))
            return false;

        PayLoad.gridID = pos;
        GridContainer.Instance.AddUnit(PayLoad);

        PayLoad = null;

        StopMoveable();
        SetMovedToken();

        return true;
    }
}
