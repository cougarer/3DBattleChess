using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class LightTank : Vehicle
{
    public static int Price = 7000;

    void Start () {
		
	}

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/LightTankEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Unit/Materials/LightTankFriendly");
        base.SetFriendly();
    }
}
