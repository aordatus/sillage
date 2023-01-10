using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum ChapterStatus
{
    Locked,
    Unlocked,
    Completed,
}

public class ChapterDisplay : MonoBehaviour
{
    [Header("Box")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI status;
    
    [Header("Chapters")]
    [SerializeField] Transform content;
    [SerializeField] Sprite locked;
    [SerializeField] Sprite unlocked;
    [SerializeField] Sprite completed;

    private List<GameObject> chapterBoxes = new List<GameObject>() { };
    private LogDatabase LogDB;
    private ChapterInBox currentCIB;

    [Header("Loader")]
    [SerializeField] Loader Ld;

    private void Awake()
    {
        LogDB = GameObject.FindGameObjectWithTag("LogDB").GetComponent<LogDatabase>();

        foreach(Transform t in content)
        {
            chapterBoxes.Add(t.gameObject);
        }
    }

    private void Start()
    {
        int a = 0;
        foreach(GameObject go in chapterBoxes)
        {
            ChapterInBox CIB = go.GetComponent<ChapterInBox>();

            var chapter = LogDB.selectedLogData.chaptersSaved.Find(x => x.title.ToLower() == CIB.Title.ToLower()); ;

            if (chapter == null)
            {
                CIB.SpriteAsset = locked;
                CIB.StatusChapter = ChapterStatus.Locked;
            }
            else
            {
                if (chapter.isCompleted)
                {
                    CIB.Score = chapter.score;
                    CIB.SpriteAsset = completed;
                    CIB.StatusChapter = ChapterStatus.Completed;
                }
                else
                {
                    CIB.SpriteAsset = unlocked;
                    CIB.StatusChapter = ChapterStatus.Unlocked;
                }
            }

            CIB.Init();

            if (a == 0)
            {
                onClickChapter(CIB);
            }
            a++;

        }
    }

    public void onClickChapter(ChapterInBox CIB)
    {
        title.text = CIB.Title;
        description.text = CIB.Description;
        if (CIB.StatusChapter == ChapterStatus.Locked)
        {
            status.text = "Locked";
        }
        else
        {
            if (CIB.StatusChapter == ChapterStatus.Completed)
            {
                status.text = $"Finished\n Score: {CIB.Score}";
            }
            else
            {
                currentCIB = CIB;
                status.text = "Play";
            }
        }

        foreach(GameObject go in chapterBoxes)
        {
            Button xButton = go.GetComponent<Button>();
            if(xButton != CIB.button)
            {
                xButton.interactable = true;
            }
            else
            {
                xButton.interactable = false;
            }
        }
    }

    public void Reloading()
    {
        Ld.Reloading();
    }

    private void Shift(string nextScene)
    {
        try
        {
            Destroy(GameObject.FindGameObjectWithTag("Music").gameObject);
        }
        catch { }


        Ld.LoadingScene(nextScene, currentCIB);

    }


    public void ClickGame()
    {
        if(status.text == "Play")
        {
            string sceneName = currentCIB.Title;

            if (LogDB.selectedLogData.isEaso)
            {
                sceneName += " (Easo)";
            }
            else
            {
                sceneName += " (Vriwon)";
            }
            Shift(sceneName);
        }

    }
}   
