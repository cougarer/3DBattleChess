using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Artillery : Vehicle
{
    public static int Price = 6000;

    void Start ()
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ArtilleryEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ArtilleryFriendly");
        base.SetFriendly();
    }

    protected override List<Point> SetAttackRange()
    {
        virtualRange = new List<Point>();

        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
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
