using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Chopter : Unit {

    public static int Price = 9000;

    void Start ()
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ChopterEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ChopterFriendly");
        base.SetFriendly();
    }

}
