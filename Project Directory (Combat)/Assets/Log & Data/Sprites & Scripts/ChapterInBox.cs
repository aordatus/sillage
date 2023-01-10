using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChapterInBox : MonoBehaviour
{

    [SerializeField] private string title;
    [SerializeField] private string settingName;
    [TextArea][SerializeField] private string descrition;

    public string Description
    {
        get { return descrition; }
    }
    public string Title
    {
        get { return title; }
    }
    public string SettingName
    {
        get { return settingName; }
    }
    public Sprite SpriteAsset { get; set; }
    public ChapterStatus StatusChapter { get; set; }
    public int Score { get; set; }
    public Button button { get; set; }

    private TextMeshProUGUI textField;
    private ChapterDisplay chapterDisplay;
    public void Init()
    {
        chapterDisplay = GameObject.FindGameObjectWithTag("DisplayBox").GetComponent<ChapterDisplay>();

        textField = this.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        textField.text = Title;
        SpriteAsset = this.GetComponent<Image>().sprite = SpriteAsset;
        button = this.GetComponent<Button>();
        button.onClick.AddListener(delegate { chapterDisplay.onClickChapter(this); });
    }
}
