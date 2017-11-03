using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Bomber : Plane
{

    public static int Price = 20000;

    void Start ()
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/BomberEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/BomberFriendly");
        base.SetFriendly();
    }
}
