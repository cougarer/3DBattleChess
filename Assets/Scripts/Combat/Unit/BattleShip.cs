using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class BattleShip : Unit {

    public static int Price = 25000;

    void Start ()
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/BattleShipEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/BattleShipFriendly");
        base.SetFriendly();
    }
}
