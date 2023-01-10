using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Character
{
    #region Custom for Editing - Base, Stage, Abilties
    [SerializeField] CharacterBase charBase;
    [SerializeField] int stage = 1;

    public CharacterBase CharBase { get { return charBase; } }
    public int Stage { get { return stage; } }
    #endregion

    //IMPORTANT 
    public int HP { get; set; }
    public int CP { get; set; }
    public int Initiation { get; set; }
    public List<Ability> Abilities { get; set; }

    public Dictionary<Stat, int> CharacterStats { get; private set; }
    public Dictionary<Stat, int> CharacterStatFlatBonuses { get; private set; }
    public Queue<string> PassiveChanges { get; private set; } = new Queue<string>();
    public Condition Disable { get; private set; }
    public int DisableTime { get; set; }
    public Condition VolatileDisable { get; private set; }
    public int VolatileDisableTime { get; set; }

    //HELPERS
    public bool HPchanged { get; set; }
    public Ability currentMove { get; set; }
    public Slider slider { get; set; }

    public void Init(CharacterBase CB = null)
    {
        if(CB != null) { charBase = CB; }

        CheckMaxStage();

        //GenerateMoves
        GenerateMoves();

        //Generate Stats
        CalculateStats();
        HPRefill();
        CPRefill();
        Initiation = Mathf.FloorToInt(Random.Range(1, 10) + charBase.InitialInitiation);

    }

    public void InitForBattle()
    {
        CalculateStats();
        Initiation = Mathf.FloorToInt(Random.Range(1, 10) + charBase.InitialInitiation);
    }

    private void CheckMaxStage()
    {
        if(Stage > CharBase.MaxStage)
        {
            stage = CharBase.MaxStage;
        }   
    }
    public void HPRefill()
    {
        HP = MaxHitPoints;
    }
    public void CPRefill()
    {
        CP = MaxCharmPoints;
    }
    private void GenerateMoves()
    {
        Abilities = new List<Ability>();
        Abilities.Clear();
        foreach (var ability in charBase.LearnableAbility)
        {
            if (ability.Stage <= Stage) { Abilities.Add(new Ability(ability.ABase)); }
        }
    }

    public string GetStatsText
    {
        get
        {
            string st = 
                  "Might: " + Might + "\n"
                + "Dexterity: " + Dexterity + "\n"
                + "Charm: " + Charm + "\n"
                + "Regen: " + Regeneration + "\n"
                + "AC: " + ArmorClass + "\n"
                + "CP: " + CP + "\n"
                + "HP: " + HP + "\n"
                + "Accuracy Boost: " + CharacterStatFlatBonuses[Stat.Accuracy] + "\n"
                    ;

            return st;
        }
    }

    private void CalculateStats()
    {
        //MAIN
        CharacterStats = new Dictionary<Stat, int>();
        CharacterStats.Add(Stat.Might, Mathf.FloorToInt(CharBase.InitialMight));
        CharacterStats.Add(Stat.Dexterity, Mathf.FloorToInt(CharBase.InitialDexterity));
        CharacterStats.Add(Stat.Charm, Mathf.FloorToInt(CharBase.InitialCharm));

        ResetStatBonuses();
        Disable = null;
        VolatileDisable = null;

        //MORE  
        MaxHitPoints = Mathf.FloorToInt(CharBase.InitialMight * 4);
        MaxCharmPoints = Mathf.FloorToInt(CharBase.InitialCharm * 2);
        Regeneration = Mathf.FloorToInt(CharBase.InitialMight);
        ArmorClass = Mod(CharBase.InitialDexterity) + 10;
    }

    private void ResetStatBonuses()
    {
        CharacterStatFlatBonuses = new Dictionary<Stat, int>()
        {
            {Stat.Might, 0},
            {Stat.Dexterity, 0},
            {Stat.Charm, 0},

            //More
            {Stat.Accuracy, 0},

        };
    }

    private int GetStat(Stat stat)
    {
        int statVal = CharacterStats[stat];

        int boost = CharacterStatFlatBonuses[stat];

        return statVal + boost;
    }

    public int MaxHitPoints { get; private set; }
    public int MaxCharmPoints { get; private set; }
    public int Regeneration { get; private set; }
    public int ArmorClass { get; private set; }

    public int Might
    {
        get { return GetStat(Stat.Might); }
    }

    public int Dexterity
    {
        get { return GetStat(Stat.Dexterity); }
    }

    public int Charm
    {
        get { return GetStat(Stat.Charm); }
    }
    public int Mod(float stat)
    {
        int a = Mathf.FloorToInt((stat - 10) / 2);
        if(stat == 5)
        {
            Debug.Log(a);
        }
        return a;
    }
    public int TotalMainAttribute
    {
        get
        {
            if (charBase.ModeType == ModeType.Charm)
            {
                return Charm;
            }
            else if (charBase.ModeType == ModeType.Dexterity)
            {
                return Dexterity;
            }
            else if (charBase.ModeType == ModeType.Might)
            {
                return Might;
            }
            else
            {
                return 0;
            }
        }
    }



    public void ApplyBonuses(List<StatBonus> statBonuses)
    {
        foreach(var statBonus in statBonuses)
        {
            CharacterStatFlatBonuses[statBonus.stat] += statBonus.boost;

            PassiveChanges.Enqueue(($"{CharBase.Name}'s {statBonus.stat} is Boosted by {statBonus.boost}"));
        }
    }

    public void SetDisable(ConditionID conditionId)
    {
        if (Disable != null) return; 
        Disable = ConditionsDB.Conditions[conditionId];
        Disable?.OnStart?.Invoke(this);
        PassiveChanges.Enqueue($"{CharBase.Name} {Disable.StartMessage}");
    }
    public void CureDisable()
    {
        Disable = null;
    }
    public void SetVolatileDisable(ConditionID conditionId)
    {
        if (VolatileDisable != null) return;
        VolatileDisable = ConditionsDB.Conditions[conditionId];
        VolatileDisable?.OnStart?.Invoke(this);
        PassiveChanges.Enqueue($"{CharBase.Name} {Disable.StartMessage}");
    }
    public void CureVolatileDisable()
    {
        VolatileDisable = null;
    }
    public void TakeRawDamage(int damage)
    {
        HPchanged = true;
        HP = Mathf.Clamp(HP - damage, 0, MaxHitPoints);
    }

    public DamageDetails TakeDamage(Ability ability, Character attacker)
    {
        float baseAttackDamage = DiceSystem.Dice(ability.Power, attacker);

        float percentageBonuses = 1; //Get this from somewhere 

        float flatBonuses = 0; //Get this from somewhere

        float mainAttackDamage = baseAttackDamage * percentageBonuses + flatBonuses;

        int finalAttackDamage = Mathf.Clamp(Mathf.FloorToInt((mainAttackDamage)), 0, Mathf.FloorToInt((mainAttackDamage)));

        //DAMAGE DETAILS PREPARATION
        var damageDetails = new DamageDetails()
        {
            damageTaken = finalAttackDamage
        };

        //AFTER EVERYTHING
        TakeRawDamage(finalAttackDamage);

        return damageDetails;

    }

    public void DecreaseErkaCapacity(Ability ability)
    {
        //CP -= ability.Cost;
    }

    public bool OnBeforeTurn()
    {
        bool canPerformMove = true;
        if(Disable?.OnBeforeMove != null)
        {
            if (!Disable.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        if (VolatileDisable?.OnBeforeMove != null)
        {
            if (!VolatileDisable.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        return canPerformMove;
    }

    private void RegenHealth()
    {
        HP += Regeneration;
    }

    public void OnAfterTurn()
    {
        Disable?.OnAfterTurn?.Invoke(this);
        VolatileDisable?.OnAfterTurn?.Invoke(this);
        //RegenHealth(); WHAT ABOUT THIS
    }

    public void OnBattleOver()
    {
        VolatileDisable = null;
        ResetStatBonuses();
    }   

    #region ENEMY
    //CAN MAKE A FREAKING AI HERE
    public Ability GetRandomMove(AbilityType abiltiyType )
    {
        List<Ability> typeAbilities = new List<Ability>();
        typeAbilities = Abilities.FindAll(x => x.aBase.AbilityType == abiltiyType);

        int r = Random.Range(0, typeAbilities.Count);
        int cost = DiceSystem.Dice(typeAbilities[r].Cost, this);

        if (cost > CP)
        {
            GetRandomMove(abiltiyType);
        }
        
        return Abilities[r];
    }
    #endregion
}

public class DamageDetails
{
    public int damageTaken { get; set; }
}
