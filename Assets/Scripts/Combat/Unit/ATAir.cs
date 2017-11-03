using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class ATAir : Vehicle
{

    public static int Price = 6000;

    void Start ()
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ATAirEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ATAirFriendly");
        base.SetFriendly();
    }
}
