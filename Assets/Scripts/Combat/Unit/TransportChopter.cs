using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class TransportChopter : Plane {

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
}
