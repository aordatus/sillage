using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Plot Item", menuName = "Assets/Items/CharacterPlus")]
public class CharacterPlus : ItemObject
{
    public string stat;
    public int plus;

    //MAKE THESE PRIVATE AND GETTER SETTER XD

        
    public void Awake()
    {
        TypeItem = ItemType.CharacterPlus;
    }
}