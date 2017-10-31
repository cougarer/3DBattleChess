using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Controller<T> : Singletion<T> where T :MonoBehaviour
{

    public abstract void AddGridType(Point key, GridType value);

    public abstract void DisplayGrid(Transform parent);

    public abstract void RemoveGrid(int key);

    public abstract void OnRemoveCurrentGrid();

    public abstract void OnGetCurrentGrid();

    public abstract void ChangeAndDisplayOneGrid(Point pos, GridType newType);
}