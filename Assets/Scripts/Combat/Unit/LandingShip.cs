using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class LandingShip : Unit {

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
}
