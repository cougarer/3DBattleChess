using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/11/6

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

    public bool Load(Unit u)
    {
        if (u is Men&& PayLoad==null)
        {
            PayLoad = u;

            u.StopHighLight();
            u.StopShowAttackRange();

            PayLoad.Hide();

            return true;
        }
        else
        {
            Debug.Log(u.gridType+"上不了车！");
            return false;
        }
    }

    public bool UnLoad(Point pos)
    {
        if (!this.CheckCouldMoveTo(GridContainer.Instance.TerrainDic[pos]))
            return false;

        PayLoad.Show(pos);
        PayLoad.StopMoveable();
        PayLoad.SetMovedToken();

        StopMoveable();
        SetMovedToken();

        return true;
    }
}
