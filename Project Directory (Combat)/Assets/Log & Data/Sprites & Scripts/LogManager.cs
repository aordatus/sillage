using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LogManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI console;
    [SerializeField] Button loadButton;
    [SerializeField] Button deleteButton;
    [SerializeField] Button backButton;
    [SerializeField] GameObject logBoxPrefab;
    [SerializeField] int numberOfLogBoxes;
    [SerializeField] Transform logListContent;
    [SerializeField] GameObject newOrLoadGO;
    [SerializeField] GameObject logListGO;
    [SerializeField] GameObject creation1GO;
    [SerializeField] GameObject creation2GO;
    [SerializeField] GameObject creation3GO;
    [SerializeField] GameObject deletion;
    [SerializeField] GameObject EasoButton;
    [SerializeField] GameObject VriwonButton;
    [SerializeField] bool isEaso;

    [Space(10)]
    [SerializeField] List<string> b = new List<string>();

    [Space(10)]
    [SerializeField] TextMeshProUGUI newLogInputText;

    [SerializeField] string newLogDifficulty;
    #region Difficulty Buttons
    [SerializeField] private Button[] difficultyButtons;

    public void SetAllButtonsInteractable()
    {
        foreach (Button button in difficultyButtons)
        {
            button.interactable = true;
        }
    }

    public void OnButtonClicked(Button clickedButton)
    {
        int buttonIndex = System.Array.IndexOf(difficultyButtons, clickedButton);

        if (buttonIndex == -1)
            return;

        SetAllButtonsInteractable();

        clickedButton.interactable = false;
    }
    #endregion

    [SerializeField] TextMeshProUGUI deleteInputText;

    [Space(10)]
    [SerializeField] AudioSource ButtonOneShot;
    [SerializeField] AudioClip ButtonPress;

    [Space(10)]
    [SerializeField] GameObject AdventureLogCanvas;
    [SerializeField] GameObject EasoCanvas;
    [SerializeField] GameObject VriwonCanvas;
    [SerializeField] string EasoInitiation;
    [SerializeField] string VriwonInitiation;
    //Working
    LogDatabase LogDB;
    private bool loadLogChosen;
    private bool noneSelected;

    [SerializeField] TextMeshProUGUI textDummy;



    void Awake()
    {
        LogDB = GameObject.FindGameObjectWithTag("LogDB").GetComponent<LogDatabase>();
        textDummy.text = "1";
        LogDB.Load();
        textDummy.text = "2";
        AdventureLogCanvas.SetActive(true);
        EasoCanvas.SetActive(false);
        VriwonCanvas?.SetActive(false);
        newOrLoadGO.SetActive(true);
        logListGO.SetActive(false);
        creation1GO.SetActive(false);
        creation2GO.SetActive(false);
        creation3GO.SetActive(false);
        ResetCharacter();
        textDummy.text = "3";
        Begin();
    }


    private void Start()
    {
        textDummy.text = "7";

        for (int i = 0; i < numberOfLogBoxes; i++)
        {
            Instantiate(logBoxPrefab, logListContent);
        }
        textDummy.text = "8";

    }

    private void Begin()
    {
        textDummy.text = "4";

        if (LogDB.LogBase.Count > 0)
        {
            textDummy.text = "5";

            console.text = welcomeBack();
            loadButton.gameObject.SetActive(true);
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            textDummy.text = "6";

            console.text = "Welcome to Sillage. Adventure Logs will keep track of your progress. " +
               "You can set options like difficulty and character story here.";
            loadButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
        }

        backButton.gameObject.SetActive(false);
    }

    private void LogListing(bool loadLog)
    {

        int i = 1;
        int j = LogDB.LogBase.Count;
        foreach(Transform t in logListContent)
        {
            if (i <= j)
            {
                t.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = $"{i}. {LogDB.LogBase[i-1].logName}";
            }
            else
            {
                t.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = $"{i}. Unused";
            }

            i++;
        }

        if (loadLog)
        {
            console.text = "Which slot would you like to use?";
        }
        else
        {
            console.text = "Which log would you like to load?";
        }
    }


    //BASIC

    public void OnDeleteBoxClick()
    {
        newOrLoadGO.SetActive(false);
        deletion.SetActive(true);
        console.text = "Enter the name of the log you want to delete.\nWarning: This can't be undone";
        backButton.gameObject.SetActive(true);
    }

    public void OnNoLClick(bool loadLog)
    {
        loadLogChosen = loadLog;
        newOrLoadGO.SetActive(false);
        logListGO.SetActive(true);
        LogListing(loadLog);
        backButton.gameObject.SetActive(true);
    }

    public void OnNextInputClick()
    {
        if (LogDB.LogBase.Any(x => x.logName.ToLower() == newLogInputText.text.ToLower()))
        {
            StartCoroutine(tempConsole(3, "There is already an adventure log registered with that name", creation1GO));
        }
        if (string.IsNullOrWhiteSpace(newLogInputText.text))
        {
            newLogInputText.text = "Untitled";
        }

        else
        {
            print(newLogInputText.text);
            creation2GO.SetActive(true);
            creation1GO.SetActive(false);
            console.text = "Choose the difficulty for your adventure. Note that this can't be changed later.";
        }
        
    }

    public void OnNextDifficultClick()
    {
        creation2GO.SetActive(false);
        creation3GO.SetActive(true);
        console.text = "Select the character you would like to play as.";
    }

    public void OnConfirmClick()
    {
        if (noneSelected)
        {
            StartCoroutine(tempConsole(2, "You have not selected any character", creation3GO));
            return;
        }

        LogData ld = new LogData();
        LogDB.LogBase.Add(ld);
        AdventureLogCanvas.SetActive(false);

        if (isEaso)
        {
            ld.isEaso = isEaso;
            ld.difficulty = newLogDifficulty;
            ld.logName = newLogInputText.text;
            
            var cd = new ChapterData();
            cd.title = EasoInitiation;
            ld.chaptersSaved.Add(cd);
            LogDB.selectedLogData = ld;

            EasoCanvas.SetActive(true);
        }

        else
        {
            ld.isEaso = isEaso;
            ld.difficulty = newLogDifficulty;
            ld.logName = newLogInputText.text;

            var cd = new ChapterData();
            cd.title = VriwonInitiation;
            ld.chaptersSaved.Add(cd);
            LogDB.selectedLogData = ld;
            VriwonCanvas.SetActive(true);
        }
        LogDB.Save();
        
        //Shift(LogDB.LogBase[a].currentChapter);
    }

    public void OnLogBoxClick(GameObject yourself)
    {
        StopAllCoroutines();
        ButtonOneShot.PlayOneShot(ButtonPress);
        string nameOfLogBox = yourself.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text.Remove(0,3);

        if (loadLogChosen)
        {
            if (nameOfLogBox.Contains("Unused"))
            {
                StartCoroutine(tempConsole(3, "This is an empty adventure log!", logListGO)); 
            }

            else
            {
                LogData ld = LogDB.LogBase.Find(x => x.logName == nameOfLogBox);

                AdventureLogCanvas.SetActive(false);

                LogDB.selectedLogData = ld;

                bool easo = ld.isEaso;
                if (easo)
                {
                    EasoCanvas.SetActive(true);
                }

                else
                {
                    VriwonCanvas.SetActive(true);
                }
            }
        }
        else
        {
            if (yourself.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text.Contains("Unused"))
            {
                logListGO.SetActive(false);
                creation1GO.SetActive(true);
                console.text = "Choose the name for your new log.";
            }

            else
            {
                StartCoroutine(tempConsole(3, "This is already a saved adventure log!", logListGO)); //DELETING LOGS IS NOT AVAILABLE BUT LATER IT WOULD BE... YOU WILL SAVE MULTIPLE TIMES LIKE WITCHER
            }
        }
    }

    public void OnBack()
    {
        if(creation1GO.activeInHierarchy)
        {
            newOrLoadGO.SetActive(false);
            logListGO.SetActive(true);
            creation1GO.SetActive(false);
            console.text = "Which slot would you like to use?";
        }
        else if (creation2GO.activeInHierarchy)
        {
            creation2GO.SetActive(false);
            creation1GO.SetActive(true);
            console.text = "Choose the name for your new log.";
        }
        else if (creation3GO.activeInHierarchy)
        {
            creation3GO.SetActive(false);
            creation2GO.SetActive(true);
            console.text = "Choose the difficulty for your adventure. Note that this can't be changed later.";
        }
        else if (deletion.activeInHierarchy)
        {
            newOrLoadGO.SetActive(true);
            deletion.SetActive(false);
            Begin();
        }
        else
        {
            newOrLoadGO.SetActive(true);
            logListGO.SetActive(false);
            creation1GO.SetActive(false);
            Begin();
        }

    }

    public void onDeleteClick()
    {
        var logdata = LogDB.LogBase.Find(x => x.logName.ToLower() == deleteInputText.text.ToLower());
        if(logdata != null)
        {
            LogDB.LogBase.Remove(logdata);
            OnBack();
           
        }
        else
        {
            StartCoroutine(tempConsole(3, "Name of this log doesn't exist.", deletion));
        }

        LogDB.Save();
    }

    private string welcomeBack()
    {
        string a = "Welcome Back. ";
        a += b[Random.Range(0, b.Count)];
        return a;
    }

    public void OnClickCharacter(bool EasoIs)
    {
        if (EasoIs)
        {
            isEaso = true;
            VriwonButton.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
            EasoButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            console.text = "Player Selected - Easo\n Story Selected - Not an Orphan";
        }

        else
        {
            isEaso = false;
            EasoButton.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
            VriwonButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            console.text = "Player Selected - Vriwon\n Story Selected - Curse of Faith";

        }
        noneSelected = false;
    }

    public void ResetCharacter()
    {
            VriwonButton.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
            EasoButton.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
            noneSelected = true;
    }

    IEnumerator tempConsole(int waiter, string newMessage, GameObject objectWhichShouldBeActive)
    {
        string oldMessage = console.text;
        console.text = newMessage;
        yield return new WaitForSeconds(waiter);
        if (objectWhichShouldBeActive.activeInHierarchy)
        {
            console.text = oldMessage;
        }

    }

}
