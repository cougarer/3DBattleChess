using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : TerrainBase
{
    public int CapturePoint
    {
        get { return capturePoint; }
    }
    [SerializeField]protected int capturePoint;

    public GridType CurrentCapturingUnit
    {
        get { return currentCapturingUnit; }
    }
    private GridType currentCapturingUnit;

	void Start ()
    {
		
	}

    public bool BeCapture(int captureCapability)
    {
        capturePoint -= captureCapability;
        return capturePoint < 0 ? true : false;
    }

    public override void SetEnemy()
    {
        capturePoint = 20;
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        capturePoint = 20;
        base.SetFriendly();
    }
}
