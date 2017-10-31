using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Destroyer : Unit {

    public static int Price = 14000;

    void Start () 
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/DestroyerEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/DestroyerFriendly");
        base.SetFriendly();
    }
}
