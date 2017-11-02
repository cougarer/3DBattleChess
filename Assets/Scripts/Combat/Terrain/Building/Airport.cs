using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Airport : Building
{

	void Start ()
    {
        
    }

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/AirportEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/AirportFriendly");
        base.SetFriendly();
    }

    public override void SetNeutral()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/AirportNeutral");
        base.SetNeutral();
    }

    public override void OnInstatiate()
    {
        SetHeight(0.4f);
    }

}
