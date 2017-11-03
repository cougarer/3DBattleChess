using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Men : Unit {

    public int CaptureCapablility { get { return (int)HP; } }

    public override bool CheckCouldMoveTo(TerrainBase tb)
    {
        GridType type = tb.gridType;
        if (type == GridType.Sea
            || type == GridType.Reef
            || GridContainer.Instance.UnitDic.ContainsKey(tb.gridID))
            return false;
        return true;
    }
}
