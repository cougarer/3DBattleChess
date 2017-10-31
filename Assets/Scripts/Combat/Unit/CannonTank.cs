using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class CannonTank : Unit {

    public static int Price = 21000;

    void Start ()
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/CannonTankEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/CannonTankFriendly");
        base.SetFriendly();
    }
}
