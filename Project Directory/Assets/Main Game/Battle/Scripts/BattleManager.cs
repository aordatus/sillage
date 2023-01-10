using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class BattleTile
{
    [SerializeField] private int indexSpot;
    [SerializeField] private int posX;
    [SerializeField] private int posY;
    [SerializeField] private List<Character> enemies = new List<Character>();
    public (int, int, int) Position { get { return (indexSpot, posX, posY); } }
    public List<Character> Enemies { get { return enemies; }}
}
public enum BattleState
{
    Nothing, Initiation, ActionSelection, RunningTurn, Buffer, Over
}
public enum BattleAction
{
    Ability, SwitchCharacter, UseItem, Escape
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] List<BattleTile> BattleTilesDatabase = new List<BattleTile>();

    [Header("Working")]
    private BattleState battleState;
    private BattleAction playerAction;

    [SerializeField] private List<Character> enemiesList = new List<Character>() { };

    private bool isOpen;
    public void AddEnemy(Character character) { enemiesList.Add(character); }
    public Party party { get; set; }

    //BATTLE UNITS
    private Character playerCurrent;
    private Character enemyCurrent;

    [SerializeField] GameObject LostBox;

    #region Awake & Update
    private void Awake()
    {
        BattleBox.SetActive(false);
        LostBox.SetActive(false);
        party = GameObject.FindGameObjectWithTag("Player").GetComponent<Party>();

        foreach (var Character in party.Characters)
        {
            Character.Init();
        }
    }

    private void Update()
    {
        if(battleState != BattleState.Nothing)
        {
            if (battleState == BattleState.ActionSelection)
            {
                EnableActionSelector(true);
            }

            else
            {
                EnableActionSelector(false);
            }
        }

    }

    #endregion

    #region Starting Battle from Roam/Selection
    public IEnumerator CheckBattle(Tile tile)
    {
        isOpen = false;
        foreach (BattleTile bt in BattleTilesDatabase)
        {
            if (bt.Position == tile.Position)
            {
                isOpen = true;
                enemiesList = bt.Enemies;
                UpdateBattleDisplay();
                break;
            }
        }
        while (isOpen)
        {
            yield return null;
        }
    }

    public void UpdateBattleDisplay()
    {
        battleState = BattleState.Initiation;

        BattleBox.SetActive(true);

        StartCoroutine(SetupBattle());
    }
    #endregion

    #region Turns
    private IEnumerator SetupBattle()
    {
        playerCurrent = party.getHealthyCharacter();
        playerCurrent.InitForBattle();
        enemyCurrent = enemiesList[0];
        enemyCurrent.Init();

        SetData(playerCurrent, cardSlotPlayer, cardNameTextPlayer, disableTextPlayer, sliderPlayer);
        SetData(enemyCurrent, cardSlotEnemy, cardNameTextEnemy, disableTextEnemy, sliderEnemy);

        SetOptions(playerCurrent);

        yield return TypeText($"Fight Begins! You must defeat {enemyCurrent.CharBase.Name}. \n\n\n{playerCurrent.CharBase.Name} Initiative: {playerCurrent.Initiation}\n {enemyCurrent.CharBase.Name} Initiative: {enemyCurrent.Initiation}", fastLetterPerSecond);
        yield return new WaitForSeconds(1);
        StartCoroutine(MoveSelection());
    }

    IEnumerator RunTurns(BattleAction battleAction, Ability playerMove = null)
    {
        battleState = BattleState.RunningTurn;

        if(playerAction == BattleAction.Ability)
        {
            playerCurrent.currentMove = playerMove;
            enemyCurrent.currentMove = enemyCurrent.GetRandomMove(AbilityType.Action);
            //Check who goes first
            bool playerGoesFirst = playerCurrent.Initiation >= enemyCurrent.Initiation;

            var FirstUnit = playerGoesFirst ? playerCurrent : enemyCurrent;
            var SecondUnit = playerGoesFirst ? enemyCurrent : playerCurrent;

            //First Turn
            yield return RunMove(FirstUnit, SecondUnit, FirstUnit.currentMove);
            if (battleState == BattleState.Over) { yield break; }
            yield return RunAfterTurn(FirstUnit);

            //Second Turn
            if (SecondUnit.HP <= 0) { StartCoroutine(MoveSelection()); yield break; }
            yield return RunMove(SecondUnit, FirstUnit, SecondUnit.currentMove);
            if (battleState == BattleState.Over) { yield break; }
            yield return RunAfterTurn(SecondUnit);

        }
        else
        {
            if(playerAction == BattleAction.SwitchCharacter)
            {
                //var selectedCharacter = party.Characters[x];

            }

            enemyCurrent.currentMove = enemyCurrent.GetRandomMove(AbilityType.Action);
            yield return RunMove(enemyCurrent, playerCurrent, enemyCurrent.currentMove);
            if (battleState == BattleState.Over) { yield break; }
            yield return RunAfterTurn(enemyCurrent);
        }

        yield return MoveSelection();


    }

    private IEnumerator MoveSelection()
    {
        yield return TypeText($"Choose An Action.");
        battleState = BattleState.ActionSelection;
    }

    //Calledfrom MoveOption Script
    public IEnumerator PlayerClickedOption(Ability ability)
    {
        yield return null;
        battleState = BattleState.Buffer;

        int cost = DiceSystem.Dice(ability.Cost, playerCurrent);

        if (playerCurrent.CP < cost)
        {
            yield return TypeText($"{playerCurrent.CharBase.Name} can't use {ability.aBase.Name} because they don't have enough Charm! \nCost Roll: {ability.Cost}, Result: {cost}", letterPerSecond);
            battleState = BattleState.ActionSelection;
        }
        
        else
        {
            playerCurrent.CP = Mathf.Clamp(playerCurrent.CP - cost, 0, playerCurrent.CP);
            yield return RunTurns(BattleAction.Ability, ability);
        }
        
    }

    private IEnumerator BattleEnd(bool won)
    {
        battleState = BattleState.Over;

        if (won)
        {
            yield return TypeText($"Fight Ends, You Won!");
        }
        else
        {
            yield return TypeText($"Fight Ends, You Lost!");

        }

        party.Characters.ForEach(p => p.OnBattleOver());

        yield return new WaitForSeconds(3);
        BattleBox.SetActive(false);
        if (!won)
        {
            LostBox.SetActive(true);
        }
        isOpen = false;
    }

    private IEnumerator RunMove(Character source, Character target, Ability ability)
    {
        bool canRunMove = source.OnBeforeTurn();
        if (!canRunMove)
        {
            yield return ShowPassiveChanges(source);
            yield return UpdateHP(source.slider, source.HP, source);
            yield break;
        }
        yield return ShowPassiveChanges(source);

        yield return TypeText($"{source.CharBase.Name} used {ability.aBase.Name}", letterPerSecond);

        if(CheckIfMoveHits(ability, source, target))
        {
            if (ability.aBase.AbilityType == AbilityType.Passive) {   yield return RunMoveEffects(ability.aBase.Effect, source, target, ability.aBase.Target); }

            else
            {
                source.DecreaseErkaCapacity(ability);
                var damageDetails = target.TakeDamage(ability, source);
                yield return UpdateHP(target.slider, target.HP, target);
                yield return ShowDamageDetails(damageDetails);
            }


            if(ability.aBase.SecondaryEffects != null && ability.aBase.SecondaryEffects.Count > 0 && target.HP > 0)
            {
                foreach(var secondary in ability.aBase.SecondaryEffects)
                {
                    if(UnityEngine.Random.Range(1,101) <= secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary, source, target, secondary.Target);
                    } 
                }
            }

            if (target.HP <= 0)
            {
                yield return TypeText($"{target.CharBase.Name} Fainted");
                yield return CheckForBattleOver(target);
            }


        }

        else
        {
            yield return TypeText($"{source.CharBase.Name}'s {ability.aBase.Name} missed");
        }
    }

    private IEnumerator CheckForBattleOver(Character faintedUnit)
    {
        //PLAYER FELL
        if (party.Characters.Contains(faintedUnit))
        {
            var nextCharacter = party.getHealthyCharacter();
            if (nextCharacter != null)
            {
                playerCurrent = nextCharacter;
                cardSlotPlayer.sprite = playerCurrent.CharBase.CardImage;
                cardNameTextPlayer.text = playerCurrent.CharBase.Name;
                SetSlider(sliderPlayer, playerCurrent);
                SetOptions(playerCurrent);

                yield return TypeText($"Your next allied combatant is now here! You must defeat {enemyCurrent.CharBase.Name}", fastLetterPerSecond);
                yield return new WaitForSeconds(1);

                battleState = BattleState.RunningTurn;
            }
            else
            {
                StartCoroutine(BattleEnd(false));
            }
        }
        
        //ENEMY FELL
        else
        {
            var nextCharacter = enemiesList.Find(x => x.HP > 0);
            if (nextCharacter != null)
            {
                enemyCurrent = nextCharacter;
                cardSlotEnemy.sprite = enemyCurrent.CharBase.CardImage;
                cardNameTextEnemy.text = enemyCurrent.CharBase.Name;
                SetSlider(sliderEnemy, enemyCurrent);

                yield return TypeText($"Next enemy has arrived for battle! You must defeat {enemyCurrent.CharBase.Name}", fastLetterPerSecond);
                yield return new WaitForSeconds(1);

                battleState = BattleState.RunningTurn;
            }
            else
            {
                StartCoroutine(BattleEnd(true));
            }
        }
    }

    #endregion

    #region Tools
    [Header("Display")]
    [SerializeField] private GameObject BattleBox;
    [SerializeField] private TextMeshProUGUI consoleText;
    [Space(10)]
    [SerializeField] private Image cardSlotPlayer;
    [SerializeField] private TextMeshProUGUI cardNameTextPlayer;
    [SerializeField] private Slider sliderPlayer;
    [SerializeField] private Transform optionsTransformPlayer;
    [SerializeField] private Transform optionPrefab;
    [Space(10)]
    [SerializeField] private Image cardSlotEnemy;
    [SerializeField] private TextMeshProUGUI cardNameTextEnemy;
    [SerializeField] private Slider sliderEnemy;
    [SerializeField] private TextMeshProUGUI disableTextPlayer;
    [SerializeField] private TextMeshProUGUI disableTextEnemy;

    IEnumerator RunAfterTurn(Character source)
    {
        source.OnAfterTurn();
        yield return ShowPassiveChanges(source);
        yield return UpdateHP(source.slider, source.HP, source);


        if (source.HP <= 0)
        {
            yield return TypeText($"{source.CharBase.Name} Fainted");
            yield return CheckForBattleOver(source);
        }

        SetDisableText(playerCurrent, disableTextPlayer);
        SetDisableText(enemyCurrent, disableTextEnemy);
    }

    bool CheckIfMoveHits(Ability ability, Character source, Character target)
    {
        if (ability.aBase.AlwaysHits)
        {
            return true;
        }

        //float moveAccuracy = ability.Accuracy;

        int accuracy = source.CharacterStatFlatBonuses[Stat.Accuracy];

        //moveAccuracy += accuracy;

        //return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
        return true;
    }

    private void SetData(Character character, Image CardSlotImage, TextMeshProUGUI CardNameText, TextMeshProUGUI CardDisableText, Slider slider)
    {
        CardSlotImage.sprite = character.CharBase.CardImage;
        CardNameText.text = character.CharBase.Name;
        SetSlider(slider, character);

        SetDisableText(character, CardDisableText);
    }

    private void SetDisableText(Character character, TextMeshProUGUI disableText)
    {
        if(character.Disable == null)
        {
            disableText.text = "";
        }
        else
        {
            disableText.text = character.Disable.Name;
        }
    }

    private IEnumerator RunMoveEffects(MoveEffects effects, Character source, Character target, TargettingTypes MoveTarget)
    {
        if (effects.Bonuses != null)
        {
            if (MoveTarget  == TargettingTypes.TargetSelf)
            {
                source.ApplyBonuses(effects.Bonuses);
            }
            else
            {
                target.ApplyBonuses(effects.Bonuses);
            }
        }

        //Disable
        if(effects.Disable != ConditionID.none)
        {
            target.SetDisable(effects.Disable);
        }

        if (effects.VolatileDisable != ConditionID.none)
        {
            target.SetVolatileDisable(effects.VolatileDisable);
        }

        yield return ShowPassiveChanges(target);
        yield return ShowPassiveChanges(source);
    }

    private IEnumerator ShowPassiveChanges(Character character)
    {
        while(character.PassiveChanges.Count > 0)
        {
            var message = character.PassiveChanges.Dequeue();
            yield return TypeText(message);
        }
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        yield return TypeText($"Final Damage: {damageDetails.damageTaken}");
    }

    private void EnableActionSelector(bool enabled)
    {
        foreach (Transform t in optionsTransformPlayer)
        {
            t.gameObject.GetComponent<Button>().interactable = enabled;
        }
    }

    private void SetSlider(Slider slider, Character character)
    {
        character.slider = slider;
        slider.maxValue = character.MaxHitPoints;
        slider.value = character.HP;
    }

    private IEnumerator UpdateHP(Slider slider, float newHp, Character character)
    {
        if (character.HPchanged)
        {
            float curHp = slider.value;
            float changeAmount = curHp - newHp;

            while (curHp - newHp > Mathf.Epsilon)
            {
                curHp -= changeAmount * Time.deltaTime;
                slider.value = curHp;
                yield return null;
            }

            slider.value = newHp;
            character.HPchanged = false;
        }
    }

    [SerializeField] int letterPerSecond;
    [SerializeField] int fastLetterPerSecond;

    private IEnumerator TypeText(string text, int customSpeed = 0)
    {
        consoleText.text = "";
        foreach(var letter in text.ToCharArray())
        {
            consoleText.text += letter;
            if (customSpeed == 0)
            {
                yield return new WaitForSeconds(1f / letterPerSecond);

            }
            else
            {
                yield return new WaitForSeconds(1f / customSpeed);

            }
        }

        yield return new WaitForSeconds(1f);

    }

    private void SetOptions(Character character)
    {
        foreach (Transform t in optionsTransformPlayer)
        {
            Destroy(t.gameObject);
        }
        foreach(Ability ability in character.Abilities)
        {
            GameObject go = Instantiate(optionPrefab, optionsTransformPlayer).gameObject;
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ability.aBase.Name;
            go.GetComponent<MoveOption>().ability = ability;
        }
    }
    #endregion
}

