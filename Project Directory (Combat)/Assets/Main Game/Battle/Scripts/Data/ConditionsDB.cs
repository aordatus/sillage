using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {


        {
            ConditionID.poison,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurn = (Character character) =>
                {
                    character.TakeRawDamage(character.MaxHitPoints/8);
                    character.PassiveChanges.Enqueue($"{character.CharBase.Name} hurt themselves due to poison");
                }
            }
        },

        //Volatile
        {
            ConditionID.slow,
            new Condition()
            {
                Name = "Slow",
                StartMessage = "has been slowed"
            }
        },
        {
            ConditionID.shackle,
            new Condition()
            {
                Name = "Shackle",
                StartMessage = "has been shackled",
                OnBeforeMove = (Character character) =>
                {
                    if(Random.Range(1,5) == 1)
                    {
                        character.CureDisable();
                        character.PassiveChanges.Enqueue($"{character.CharBase.Name} is not shackled anymore");
                        return true;
                    }
                    return false;
                }
            }
        },
        {
            ConditionID.sleep,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has falled asleep",
                OnStart = (Character character) =>
                {
                    //Sleep for 1-3 turns
                    character.VolatileDisableTime = Random.Range(1,4);
                    Debug.Log($"Will be asleep for {character.VolatileDisableTime} abilities");
                },

                OnBeforeMove = (Character character) =>
                {
                    if(character.VolatileDisableTime <= 0)
                    {
                        character.CureVolatileDisable();
                        character.PassiveChanges.Enqueue($"{character.CharBase.Name} woke Up");
                        return true;

                    }
                    character.VolatileDisableTime--;
                    character.PassiveChanges.Enqueue($"{character.CharBase.Name} is sleeping");
                    return false;
                }
            }
        }
    };

}

public enum ConditionID
{
    none, slow, silence, sleep, shackle, hex, taunt, fear, blind, disarm, etheral, berserk,
    poison
}
