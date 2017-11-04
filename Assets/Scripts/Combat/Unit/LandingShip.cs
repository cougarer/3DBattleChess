using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class LandingShip : Ship,ITransport
{

    public static int Price = 12000;

    void Start () {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/LandingShipEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/LandingShipFriendly");
        base.SetFriendly();
    }

    public Unit PayLoad
    {
        get;
        set;
    }

    public bool Load(Unit u)
    {
        if (u is Men||u is Vehicle)
        {
            PayLoad = u;

            u.StopHighLight();
            u.StopShowAttackRange();

            GridContainer.Instance.UnitDic.Remove(u.gridID);
            u.gridID = null;
            Destroy(u.gameObject);

            return true;
        }
        return false;
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
