using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plot Item", menuName = "Assets/Items/Plot")]
public class PlotItem : ItemObject
{
    public void Awake()
    {
        TypeItem = ItemType.Plot;
    }
}
