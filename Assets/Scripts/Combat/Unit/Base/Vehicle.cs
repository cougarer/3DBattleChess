using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : Unit {

    public override bool CheckCouldMoveTo(TerrainBase tb)
    {
        GridType type=tb.gridType;
        if (type == GridType.Mountain
            || type == GridType.Sea
            || type == GridType.Reef
            || GridContainer.Instance.UnitDic.ContainsKey(tb.gridID))
            return false;
        return true;
    }
}
