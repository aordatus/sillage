using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Assets/Ability")]
public class AbilityBase : ScriptableObject
{
    #region General
    [SerializeField] new string name;
    [SerializeField] AbilityType abilityType;
    [TextArea]
    [SerializeField] string description;
    public string Name { get { return name; } }
    public AbilityType AbilityType { get { return abilityType; } }
    public string Description { get { return description; } }
    #endregion

    #region Working 
    [SerializeField] string basePower;
    [SerializeField] string baseCost;
    [SerializeField] bool alwaysHits;
    [SerializeField] bool isHeal;
    [SerializeField] TargettingTypes target;
    [SerializeField] MoveEffects effect;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    public string Cost { get { return baseCost; } }
    public string Power { get { return basePower; } }
    public bool AlwaysHits { get { return alwaysHits; } }
    public bool IsHeal { get { return isHeal; } }
    public TargettingTypes Target { get { return target; } }
    public MoveEffects Effect { get { return effect; } }
    public List<SecondaryEffects> SecondaryEffects { get { return secondaryEffects; } }
    #endregion
}

public enum AbilityType
{
    Passive,
    Action,
    Bonus,
    Auto
}

public enum TargettingTypes
{
    NoTarget,
    TargetUnit,
    TargetUnits,
    TargetSelf
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBonus> bonuses;
    [SerializeField] ConditionID disable;
    [SerializeField] ConditionID volatileDisable;

    public List<StatBonus> Bonuses { get { return bonuses; } }
    public ConditionID Disable { get { return disable; } }
    public ConditionID VolatileDisable { get { return volatileDisable; } }
}

[System.Serializable]
public class SecondaryEffects: MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] TargettingTypes target;

    public int Chance { get { return chance; } }
    public TargettingTypes Target { get { return target; } }
}

[System.Serializable]
public class StatBonus
{
    public Stat stat;
    public int boost;
}

public static class DiceSystem
{
    public static int Dice(string xdyyttbb, Character c) //2d10n4 2 times d10 normal type +4 flat bonus
    {
        int amount1 = 0;

        if (xdyyttbb == "0")
        {
            return amount1;
        }

        try
        {
            int x = int.Parse(xdyyttbb[0].ToString());
            int y = int.Parse(xdyyttbb.Substring(2,2));

            if (xdyyttbb[4] == 'n') //Normal
            {
                for (int i = 0; i < x; i++)
                {
                    amount1 += Random.Range(1, y + 1);
                }

                if(xdyyttbb[5] == 't') //TotalMain
                {
                    amount1 += c.Mod(c.TotalMainAttribute);
                }
                else if (xdyyttbb[5] == 'd') //Dex
                {
                    amount1 += c.Mod(c.Dexterity);
                }
                else if (xdyyttbb[5] == 'c') //Charm
                {
                    amount1 += c.Mod(c.Might);
                }
                else if (xdyyttbb[5] == 'm') //Might
                {
                    amount1 += c.Mod(c.Charm);
                }
                else if (xdyyttbb[5] == 'n') //None
                {
                    amount1 += 0;
                }
            }
        }
        catch
        {
            Debug.Log($"Ability {xdyyttbb} Not Working.");   
        }

        return amount1 + int.Parse(xdyyttbb.Substring(6, 2));
    }
}



