using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Plot, //Can't Be Consumed //Only Possession
    CharacterPlus, //Healing Empowering Etc
    CharacterBonus //Boosting, Modifier
}

public class ItemObject : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private ItemType typeItem;
    [SerializeField] private bool throwable;
    [SerializeField] private string description;
    [SerializeField] private Sprite sprite;

    public string Name { get { return name; } }
    public bool Throwable { get { return throwable; } }
    public ItemType TypeItem
    {
        get { return typeItem; }
        set { value = typeItem; }
    }
    public string Description { get { return description; }  }
    public Sprite Sprite { get { return sprite; } }

}
