using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionDisplay;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private GameObject characterSelectionBox;
    [SerializeField] private TextMeshProUGUI selectionConsole;

    [SerializeField] private CharacterBase[] allCharacters;

    [SerializeField] BattleManager battleManager;

    public List<CharacterBase> PlayerParty = new List<CharacterBase> { };
    public List<CharacterBase> EnemyParty = new List<CharacterBase> { };

    private void Start()
    {
        foreach(CharacterBase cb in allCharacters)
        {
            BoxCharacter boxChar = Instantiate(characterSelectionBox, playerTransform).GetComponent<BoxCharacter>();
            boxChar.CB = cb;
            boxChar.isPlayer = true;
            boxChar.Init();

            BoxCharacter boxChar2 = Instantiate(characterSelectionBox, enemyTransform).GetComponent<BoxCharacter>();
            boxChar2.CB = cb;
            boxChar2.isPlayer = false;
            boxChar2.Init();

        }

        UpdateText();
    }

    public void UpdateText()
    {
        selectionConsole.text = $"Player Team: {string.Join(", ", PlayerParty)}\n".Replace(" (CharacterBase)", "");
        selectionConsole.text += $"\nVS\n";
        selectionConsole.text += $"\nEnemy Team: {string.Join(", ", EnemyParty)}".Replace(" (CharacterBase)", "");

    }
    public void DoneSelection()
    {
        if (PlayerParty.Count > 0 && EnemyParty.Count > 0)
        {
            Party party = GameObject.FindGameObjectWithTag("Player").GetComponent<Party>();

            foreach (CharacterBase CB in PlayerParty)
            {
                Character character = new Character();
                character.Init(CB);
                party.AddCharacter(character);
            }

            foreach (CharacterBase CB in EnemyParty)
            {
                Character character = new Character();
                character.Init(CB);
                battleManager.AddEnemy(character);
            }

        }

        else
        {
            print("Select More");
        }
    }
    public void StartGame()
    {
        selectionDisplay.SetActive(false);
        battleManager.UpdateBattleDisplay();
    }
}
