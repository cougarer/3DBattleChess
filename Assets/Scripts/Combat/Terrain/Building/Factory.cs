using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Factory : Building
{

	void Start ()
    {
    }

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/FactoryEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/FactoryFriendly");
        base.SetFriendly();
    }

    public override void SetNeutral()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/FactoryNeutral");
        base.SetNeutral();
    }

    public override void OnInstatiate()
    {
        SetHeight(0.4f);
    }

}
