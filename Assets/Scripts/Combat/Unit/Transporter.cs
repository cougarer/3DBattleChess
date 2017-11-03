using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Transporter : Vehicle, ITransport
{

    public static int Price = 5000;

	void Start () 
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/TransporterEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/TransporterFriendly");
        base.SetFriendly();
    }

    protected override List<Point> SetAttackRange()
    {
        virtualRange = new List<Point>();
        return virtualRange;
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
