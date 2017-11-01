using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Transporter : Unit {

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
}
