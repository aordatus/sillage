using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameState
{
    initial, playerTurn, enemyTurn, pause, busy, battle
}

public class StateManager : MonoBehaviour//, ISavable
{
    private GameState theGameState;
    public GameState lastGameState;

    [SerializeField] private TextMeshProUGUI texto;
    [SerializeField] private int enemyWaiter;

    public GameState ReadGameState { get { return theGameState; } }
    
    private NPCManager npcManager;
    private SettingManager settingManager;
    private PlayerMovement playerMovement;

    [SerializeField] private List<Button> taskbarImpactOptions;
    [SerializeField] private List<Button> taskbarHardImpactOptions;

    private void Awake()
    {
        npcManager = this.gameObject.GetComponent<NPCManager>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        settingManager = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingManager>();
        
    }

    private void Start()
    {
        PlacePlayer();
    }


    #region PlayerSpawn
    public void PlacePlayer()
    {
        //Order 1
        settingManager.turningOnSpot(settingManager.startingPosition.Item1);

        //Order 2
        playerMovement.SetPosition(settingManager.startingPosition.Item2, settingManager.startingPosition.Item3);
    }

    public void ChangeScene(string scenName, int spotIndex, int iniX, int iniY)
    {
        /* WHAT?
        PlayerData.spotIndex = spotIndex;
        PlayerData.spawn_x = iniX;
        PlayerData.spawn_y = iniY;
        */ 
        SceneManager.LoadScene(scenName);
    }
    #endregion
    
    private void Update()
    {
        PlayerActionHandler(theGameState != GameState.playerTurn);
    }

    public IEnumerator EnemyTurn()
    {
        theGameState = GameState.enemyTurn;
        texto.text = "NPC's Turn";
        foreach (var EM in npcManager.activeNPC)
        {
            if(EM != null)
            {
                yield return EM.TryToDoSomething();
            }
        }

        StartCoroutine(PlayerTurn());
    }

    public IEnumerator PlayerTurn()
    {
        playerMovement.ShowPossibleMovement();  

        theGameState = GameState.playerTurn;
        texto.text = "PC's Turn";
        yield return new WaitForSeconds(0.35f);
    }

    private void PlayerActionHandler(bool block)
    {
        playerMovement.MovingAllowed = !block;
        foreach(Button but in taskbarHardImpactOptions)
        {
            but.GetComponent<Button>().interactable = !block;
        }
    }

    public IEnumerator GoBusy()
    {
        theGameState = GameState.busy;
        texto.text = "Busy";
        yield return new WaitForSeconds(0.65f);
    }

    public void GoPause()
    {
        lastGameState = ReadGameState;
        theGameState = GameState.pause;
    }

    public void GoResume()
    {
        theGameState = lastGameState;
        if(theGameState == GameState.enemyTurn)
        {
            StartCoroutine(EnemyTurn());
        }
    }

    public void GoBattle()
    {
        theGameState = GameState.battle;
        foreach(Button but in taskbarImpactOptions)
        {
            if (taskbarHardImpactOptions.Contains(but))
            {
                but.interactable = true;
            }
            else
            {
                but.interactable = false;
            }
        }
    }

    /*
    public object CaptureState()
    {
        float[] pos = new float[] { playerMovement.Position.Item1, playerMovement.Position.Item2, playerMovement.Position.Item3 };
        return pos;
    }

    public void RestoreState(object state)
    {
        var position = ((float[])state);
        PlacePlayer();
    }
    */
}
