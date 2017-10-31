using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Rockets : Unit {

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
}
