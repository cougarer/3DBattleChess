using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building : TerrainBase
{
    protected int oldCapturePoint;

    public int capturePoint;

    public Unit CurrentCapturingUnit
    {
        get { return currentCapturingUnit; }
    }
    private Unit currentCapturingUnit;

	void Start ()
    {
        Debug.Log(oldCapturePoint);
	}

    public void BeCapture(CaptureUnit captureUnit)
    {
        if (currentCapturingUnit != captureUnit)   //如果来占领的人换了，就得重新占领
        {
            currentCapturingUnit = captureUnit;
            capturePoint = oldCapturePoint;
            Debug.Log(oldCapturePoint);
        }

        currentCapturingUnit = captureUnit;

        capturePoint -= captureUnit.CaptureCapablility;

        if (capturePoint <= 0)  //被占领了
        {
            capturePoint = oldCapturePoint;
            switch (captureUnit.Side)
            {
                case SideType.Enemy:
                    {
                        SetEnemy();
                    }
                    break;
                case SideType.Friendly:
                    {
                        SetFriendly();
                    }
                    break;
            }
        }
    }
    public void ResetCapturePoint()
    {
        capturePoint = oldCapturePoint;
    }

    public override void OnInstatiate()
    {
        oldCapturePoint = capturePoint;    //记录原有的占领点数
    }
}
