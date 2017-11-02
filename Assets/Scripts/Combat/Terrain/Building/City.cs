using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class City : Building
{

	void Start ()
    {
        
    }

    public override void SetEnemy()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/CityEnemy");
        base.SetEnemy();
    }

    public override void SetFriendly()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/CityFriendly");
        base.SetFriendly();
    }

    public override void SetNeutral()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/Building/Materials/CityNeutral");
        base.SetNeutral();
    }

    public override void OnInstatiate()
    {
        SetHeight(0.4f);
        base.OnInstatiate();

        #region 统计每个阵营有几个city，方便加钱
        if (!GridContainer.isEditorMode)
        {
            switch (Side)
            {
                case SideType.Friendly:
                    {
                        CombatController.p1CitysCount++;
                        break;
                    }
                case SideType.Enemy:
                    {
                        CombatController.p2CitysCount++;
                        break;
                    }
            }
        }
        #endregion
    }

}
