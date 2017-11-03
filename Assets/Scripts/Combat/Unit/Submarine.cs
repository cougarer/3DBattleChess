using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Submarine : Ship
{

    public static int Price = 15000;

    void Start ()
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/SubmarineEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/SubmarineFriendly");
        base.SetFriendly();
    }
}
