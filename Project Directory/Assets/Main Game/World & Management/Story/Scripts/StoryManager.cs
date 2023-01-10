using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StoryManager : MonoBehaviour
{
    [Header("Story")]
    [SerializeField] private List<Block> allBlocks = new List<Block>();
    [SerializeField] private TextAsset settingStory;
    private Dictionary<string, string> idAndText = new Dictionary<string, string>();
    private PlayerMovement playerMovement;
    private SettingManager settingManager;
    [Header("Display")]
    [SerializeField] private GameObject storyBox;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite defaultIcon;
    private bool isOpen;
    private Block curretBlock;
    private void Awake()
    {
        iconImage.sprite = null;

        storyTextWork();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        settingManager = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingManager>();

        /*
        foreach (KeyValuePair<string, string> kvp in IdAndText)
        {
            Debug.Log($"Key = {kvp.Key}, Value = {kvp.Value}");
        }
        */
    }

    public IEnumerator CheckStory()
    {
        isOpen = false;
        foreach (Block block in allBlocks)
        {
            if (block.Trial(playerMovement))
            {
                curretBlock = block;
                DisplayStory(block);
                break;
            }
        }

        while (isOpen)
        {
            yield return null;
        }
        
    } 

    private void DisplayStory(Block block)
    {
        isOpen = true;
        storyBox.SetActive(true);
        displayText.text = GetText(block.Id);
        if (!iconImage.sprite)
        {
            iconImage.sprite = defaultIcon;
        }
    }

    private string GetText(string id)
    {
        if (idAndText.ContainsKey(id))
        {
            return idAndText[id];
        }
        else
        {
            print("error");
            return null;
        }
    }

    public void CloseDisplay()
    {
        isOpen = false;
        if (!settingManager.interactedBlocks.Contains(curretBlock)) //avoiding to add repeat blocks again and again
        {
            settingManager.interactedBlocks.Add(curretBlock);
        }
        curretBlock = null;
        displayText.text = "";
        iconImage.sprite = null;
        storyBox.SetActive(false);
    }

    private string Between(string STR, string FirstString, string LastString)
    {
        string FinalString;
        int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
        int Pos2 = STR.IndexOf(LastString);
        FinalString = STR.Substring(Pos1, Pos2 - Pos1);
        return FinalString;
    }

    private void storyTextWork()
    {
        var lines = settingStory.text.Split('\n');
        string id = "[z0]";

        foreach (var line in lines)
        {
            if (line.Contains("["))
            {
                id = Between(line, "[", "]");
            }

            else
            {
                if (!idAndText.ContainsKey(id))
                {
                    idAndText[id] = line;

                }
                else
                {
                    string old = idAndText[id];
                    idAndText[id] = old + "\n" + line;
                }

            }

        }
    }

}
