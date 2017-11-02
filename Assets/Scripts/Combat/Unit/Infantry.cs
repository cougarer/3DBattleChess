using System;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Infantry : CaptureUnit
{
    public static int Price = 1000;

    void Start () 
    {
        
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/InfantryEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/InfantryFriendly");
        base.SetFriendly();
    }
}
