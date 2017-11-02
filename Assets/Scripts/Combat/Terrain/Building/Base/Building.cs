using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building : TerrainBase
{
    private int oldCapturePoint;

    public int capturePoint;

    public Unit CurrentCapturingUnit
    {
        get { return currentCapturingUnit; }
    }
    private Unit currentCapturingUnit;

	void Start ()
    {
        oldCapturePoint = capturePoint;    //记录原有的占领点数
        Debug.Log(oldCapturePoint);
	}

    public void BeCapture(CaptureUnit captureUnit)
    {
        if (currentCapturingUnit != captureUnit)   //如果来占领的人换了，就得重新占领
        {
            currentCapturingUnit = captureUnit;
            capturePoint = oldCapturePoint;
        }

        currentCapturingUnit = captureUnit;

        Debug.Log(capturePoint);

        capturePoint -= captureUnit.CaptureCapablility;

        Debug.Log(captureUnit.CaptureCapablility);

        if (capturePoint <= 0)  //被占领了
        {
            capturePoint = oldCapturePoint;
            switch (captureUnit.Side)
            {
                case SideType.Enemy:
                    {
                        SetFriendly();
                    }
                    break;
                case SideType.Friendly:
                    {
                        SetEnemy();
                    }
                    break;
            }
        }
    }
    public void ResetCapturePoint()
    {
        capturePoint = oldCapturePoint;
    }
}
