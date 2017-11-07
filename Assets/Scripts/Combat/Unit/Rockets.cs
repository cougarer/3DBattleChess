using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Rockets : Vehicle
{

    public static int Price = 18000;

    void Start () {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/RocketsEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/RocketsFriendly");
        base.SetFriendly();
    }

    protected override List<Point> SetAttackRange()
    {
        virtualRange = new List<Point>();

        for (int i = -3; i <= 3; i++)
        {
            for (int j = -3; j <= 3; j++)
            {
                virtualRange.Add(new Point(i, j));
            }
        }
        virtualRange.Remove(new Point(1, 0));
        virtualRange.Remove(new Point(0, 1));
        virtualRange.Remove(new Point(-1, 0));
        virtualRange.Remove(new Point(0, -1));
        return virtualRange;
    }
}
