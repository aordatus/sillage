using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CharacterListButton : MonoBehaviour
{
    public Character character;
    public void Init()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = character.CharBase.Name;
    }
    public void OnClickCharacterSlot()
    {
        GameObject.FindGameObjectWithTag("InvManager").GetComponent<PartyManager>().DisplayCharacterInfo(character);
    }
}
