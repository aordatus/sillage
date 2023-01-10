using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Party : MonoBehaviour
{
    [SerializeField] List<Character> characters;
    public List<Character> Characters { get { return characters;  } }

    private void Awake()
    {       
        SetIcon();
    }

    public void SetIcon()
    {
        try
        {
            GetComponent<PlayerMovement>().charBase = characters[0].CharBase;
            GetComponent<SpriteRenderer>().sprite = characters[0].CharBase.IconImage;
        }
        catch
        {
            print("No error if you are building battle scene");
        }

    }

    public Character getHealthyCharacter()
    {
        return characters.Where(x => x.HP > 0).FirstOrDefault();
    }
    public void AddCharacter(Character character)
    {
        characters.Add(character);
    }
}
