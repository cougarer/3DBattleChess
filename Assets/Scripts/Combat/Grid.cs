using UnityEngine;
using System.Collections;

//Author: MaxLykoS
//UpdateTime: 2017/10/22

public abstract class Grid : MonoBehaviour
{
    public Point gridID;
    public GridType gridType;
    public SideType Side = SideType.Neutral;

    protected bool bHighLight = false;

    /// <summary>
    /// 判断是否为地形类型
    /// </summary>
    /// <returns></returns>
    public bool IsTerrain()
    {
        if (this is TerrainBase)
            return true; 
        else
            return false;
    }

    /// <summary>
    /// 阵营设为中立
    /// </summary>
    public virtual void SetNeutral()
    {
        Side = SideType.Neutral;
    }

    /// <summary>
    /// 阵营设为盟友
    /// </summary>
    public virtual void SetFriendly()
    {
        if (this is Landform)
            throw new System.Exception("地形不能设为非中立！");

        Side = SideType.Friendly;
    }

    /// <summary>
    /// 阵营设为敌人
    /// </summary>
    public virtual void SetEnemy()
    {
        if (this is Landform)
            throw new System.Exception("地形不能设为非中立！");

        Side = SideType.Enemy;
    }

    /// <summary>
    /// 设置阵营为
    /// </summary>
    /// <param name="newSide"></param>
    public virtual void SetSide(SideType newSide)
    {
        switch (newSide)
        {
            case SideType.Enemy:
                SetEnemy();
                break;
            case SideType.Friendly:
                SetFriendly();
                break;
            case SideType.Neutral:
                SetNeutral();
                break;
        }
    }

    /// <summary>
    /// 设置高度为为
    /// </summary>
    /// <param name="h"></param>
    public virtual void SetHeight(float h)
    {
        transform.Translate(0, h, 0);
    }

    /// <summary>
    /// 当物体被加载时的回调
    /// </summary>
    public abstract void OnInstatiate();

    /// <summary>
    /// 设置格子高亮
    /// </summary>
    public abstract void SetHighLight();

    /// <summary>
    /// 停止格子高亮
    /// </summary>
    public abstract void StopHighLight();

    protected abstract IEnumerator IHightLight();
}
