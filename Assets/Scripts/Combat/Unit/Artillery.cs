using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Artillery : Unit 
{
    public static int Price = 6000;

    void Start ()
    {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ArtilleryEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/ArtilleryFriendly");
        base.SetFriendly();
    }
}
