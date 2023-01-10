using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
    //[HideInInspector] public string SlotName;

    public AbilityBase aBase { get; set; }
    public string Cost { get { return aBase.Cost; } }
    public string Power { get { return aBase.Power; } }

    public Ability(AbilityBase aBase)
    {
        //this.SlotName = aBase.Name;
        this.aBase = aBase;

        /*
        foreach(StatBonus bonus in aBase.Effect.Bonuses)
        {
            bonus.boost = Mathf.FloorToInt(bonus.boost * (1 + expertise / 100));
        }
        */
    }
}

