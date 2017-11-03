using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Unit {

    public override bool CheckCouldMoveTo(TerrainBase tb)
    {
        GridType type =tb.gridType;
        if (type != GridType.Sea
            ||GridContainer.Instance.UnitDic.ContainsKey(tb.gridID))
            return false;
        return true;
    }
}
