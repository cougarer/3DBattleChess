using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class Shoal : TerrainBase
{

	void Start ()
    {

    }

    void CheckRotation()
    {
        Dictionary<Point, TerrainBase> terrainDic = GridContainer.Instance.TerrainDic;
        if (terrainDic.ContainsKey(gridID.Left()))
        {
            if (terrainDic[gridID.Left()].gridType == GridType.Sea)
            {
                RotateToLeft();
                return;
            }
        }
        if (terrainDic.ContainsKey(gridID.Right()))
        {
            if (terrainDic[gridID.Right()].gridType == GridType.Sea)
            {
                RotateToRight();
                return;
            }
        }
        if (terrainDic.ContainsKey(gridID.Up()))
        {
            if (terrainDic[gridID.Up()].gridType == GridType.Sea)
            {
                RotateToUp();
                return;
            }
        }
        if (terrainDic.ContainsKey(gridID.Down()))
        {
            if (terrainDic[gridID.Down()].gridType == GridType.Sea)
            {
                RotateToDown();
                return;
            }
        }
    }

    public override void OnInstatiate()
    {
        SetHeight(-0.3f);
        CheckRotation();
    }
}
