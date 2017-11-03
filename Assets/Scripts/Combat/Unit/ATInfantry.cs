using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class ATInfantry : Men {

    public static int Price = 3000;

    void Start ()
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ATInfantryEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ATInfantryFriendly");
        base.SetFriendly();
    }
}
