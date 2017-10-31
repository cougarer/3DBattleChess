using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class HeavyTank : Unit {

    public static int Price = 16000;

    void Start () 
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/HeavyTankEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/HeavyTankFriendly");
        base.SetFriendly();
    }
}
