using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class TransportChopter : Unit {

    public static int Price = 8000;

    void Start () 
    {
		
	}

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
}
