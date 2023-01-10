using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyManager : MonoBehaviour
{
    [SerializeField] Image displayCardImage;
    [SerializeField] TextMeshProUGUI displayCardInfo;
    [SerializeField] Button selectionButton;
    private int DisplayingCharacterIndex;

    [SerializeField] Transform charactersContent;
    [SerializeField] GameObject characterListButtonPrefab;
    private Party party;
    private StateManager stateManager;

    private void Awake()
    {
        party = GameObject.FindGameObjectWithTag("Player").GetComponent<Party>();
        stateManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<StateManager>();

        ClearCharacterList();
        CharactersInList();
        DisplayCharacterInfo(party.Characters[0]);
    }

    private void CharactersInList()
    {
        foreach (Character character in party.Characters)
        {
            CharacterListButton CLB = Instantiate(characterListButtonPrefab, charactersContent).GetComponent<CharacterListButton>();
            CLB.character = character;
            CLB.Init();
        }
    }
    private void ClearCharacterList()
    {
        foreach (Transform t in charactersContent)
        {
            Destroy(t.gameObject);
        }
    }

    public void DisplayCharacterInfo(Character character)
    {
        DisplayingCharacterIndex = party.Characters.IndexOf(character);
        displayCardImage.sprite = character.CharBase.CardImage;
        displayCardInfo.text = character.CharBase.FullDescription + "\n\n\n" + character.GetStatsText;

        if (party.Characters[0] != character)
        {
            selectionButton.interactable = true;
            selectionButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Select";
        }

        else
        {
            selectionButton.interactable = false;
            selectionButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Selected";

        }
    }
    
    public void SelectCharacter()
    {
        Swap(party.Characters, DisplayingCharacterIndex, 0);
        
        
        party.SetIcon();
        stateManager.lastGameState = GameState.enemyTurn;


        ClearCharacterList();
        CharactersInList();
        DisplayCharacterInfo(party.Characters[0]);
    }
    private  void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }


}
