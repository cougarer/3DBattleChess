using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class HQ : TerrainBase
{
    public static GameObject HQFriendly = null;
    public static GameObject HQEnemy = null;
    void Start ()
    {
        
    }

    /// <summary>
    /// HQ一加载时检查阵营类型
    /// </summary>
    public void AutoCheckSide()
    {
        if (HQFriendly != null && HQEnemy == null) HQEnemy = gameObject;  //有我没敌 则为敌
        else if (HQFriendly == null && HQEnemy != null) HQFriendly = gameObject; //有敌没我 则为我
        else if (HQFriendly == null && HQEnemy == null) HQFriendly = gameObject;  //没敌没我 则为我

        if (HQFriendly != null) HQFriendly.GetComponent<HQ>().SetFriendly();
        if (HQEnemy != null) HQEnemy.GetComponent<HQ>().SetEnemy();
    }

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/HQEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/HQFriendly");
        base.SetFriendly();
    }

    public override void SetNeutral()
    {
        throw new System.Exception("HQ不能为中立!");
    }

    public override void OnInstatiate()
    {
        SetHeight(0.8f);
    }
}
