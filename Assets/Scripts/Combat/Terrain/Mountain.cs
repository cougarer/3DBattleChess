using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Mountain : TerrainBase
{
	void Start ()
    {
        
    }

    public override void OnInstatiate()
    {
        SetHeight(0.5f);
    }

}
