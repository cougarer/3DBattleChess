using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Road : Landform
{

    void Start ()
    {
        
    }

    void CheckRotation()
    {
        Dictionary<Point, TerrainBase> terrainDic = GridContainer.Instance.TerrainDic;
        if (terrainDic.ContainsKey(gridID.Up()))
        {
            if (terrainDic[gridID.Up()].gridType == GridType.Road)
            {
                RoadRotate();
                terrainDic[gridID.Up()].RoadRotate();
                return;
            }
        }
        if (terrainDic.ContainsKey(gridID.Down()))
        {
            if (terrainDic[gridID.Down()].gridType == GridType.Road)
            {
                RoadRotate();
                terrainDic[gridID.Down()].RoadRotate();
                return;
            }
        }
    }

    public override void OnInstatiate()
    {
        CheckRotation();
    }

}
