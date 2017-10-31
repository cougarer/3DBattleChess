using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Fighter : Unit {

    public static int Price = 8000;

    void Start () 
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/FighterEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/FighterFriendly");
        base.SetFriendly();
    }
}
