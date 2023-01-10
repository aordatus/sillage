using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class PlayerSavaData
{
    public float[] position { get; set; }
    public List<Character> partyCharacters { get; set; }
    public int capacity { get; set; } = 3;
    public List<ItemObject> ownedItems { get; set; }
}

[System.Serializable]
public class ChapterData
{
    public string title { get; set; } //this will have (Easo) (Vriwon) too
    public int score { get; set; }
    public bool isCompleted { get; set; }
}


[System.Serializable]
public class LogData
{
    public string logName { get; set; }
    public string difficulty { get; set; }
    public bool isEaso { get; set; }
    public List<ChapterData> chaptersSaved { get; set; } = new List<ChapterData>() { };
    public PlayerSavaData PlayerSD { get; set; }
}

public class LogDatabase : MonoBehaviour, ISavable
{
    public List<LogData> LogBase { get; set; } = new List<LogData>();
    public LogData selectedLogData { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void Save()
    {
        SavingSystem.i.Save("SillageData");
    }

    public void Load()
    {
        SavingSystem.i.Load("SillageData");
    }

    public object CaptureState()
    {
        return LogBase;
    }

    public void RestoreState(object state)
    {
        LogBase = ((IEnumerable)state).Cast<LogData>().ToList();
    }
}
