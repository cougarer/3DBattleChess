using System;
using System.Collections;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public abstract class TerrainBase : Grid
{
    public int OilCost;
    public int DefendStar;

    /// <summary>
    /// 向左旋转
    /// </summary>
    public void RotateToLeft()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// 向右旋转
    /// </summary>
    public void RotateToRight()
    {
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    /// <summary>
    /// 向上旋转
    /// </summary>
    public void RotateToUp()
    {
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    /// <summary>
    /// 向下旋转
    /// </summary>
    public void RotateToDown()
    {
        transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    /// <summary>
    /// 道路旋转
    /// </summary>
    public void RoadRotate()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// 设置高亮
    /// </summary>
    public override void SetHighLight()
    {
        bHighLight = true;
        StartCoroutine(IHightLight());
    }
    protected override IEnumerator IHightLight()
    {
        float yRotation = transform.localEulerAngles.y;
        while (bHighLight)
        {
            transform.Rotate(2, 0, 0,Space.World);
            yield return new WaitForSeconds(0.01f);
        }
        transform.localEulerAngles = new Vector3(0, yRotation, 0);
        yield return 0;
    }
    /// <summary>
    /// 停止高亮
    /// </summary>
    public override void StopHighLight()
    {
        bHighLight = false;
    }

    /// <summary>
    /// 物体被加载出来后调用
    /// </summary>
    public override void OnInstatiate()
    {
        
    }
}