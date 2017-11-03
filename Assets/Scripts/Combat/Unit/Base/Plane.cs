using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : Unit
{
    public override bool CheckCouldMoveTo(TerrainBase tb)
    {
        return !GridContainer.Instance.UnitDic.ContainsKey(tb.gridID);
    }
}
