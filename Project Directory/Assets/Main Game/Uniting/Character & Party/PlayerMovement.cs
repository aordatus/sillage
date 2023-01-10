using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterBase charBase { get; set; }
    private Tile currentTileOccupied;
    private int currentX;
    private int currentY;

    [Header("Helpers")]
    private bool justTeleported;
    private bool movingAllowed = true;
    public bool MovingAllowed { get { return movingAllowed; } set { value = movingAllowed; } }
    public (int, int, int) Position { get { return (currentTileOccupied.IndexSpot, currentX, currentY); } }
    public Tile CurrentTile { get { return currentTileOccupied; } }

    [SerializeField] private GameObject Manager;
    [SerializeField] private Loader loader;

    private StateManager stateManager;
    private StoryManager storyManager;
    private ItemManager itemManager;
    private BattleManager battleManager;
    private NPCManager npcManager;
    private MovementMechanics movementMechanics;
    private SettingManager settingManager;
    private Party playerPartyManager;
    private int beginning = 1; //0 is no 1 is yes

    private void Awake()
    {
        movementMechanics = this.gameObject.GetComponent<MovementMechanics>();

        stateManager = Manager.GetComponent<StateManager>();
        storyManager = Manager.GetComponent<StoryManager>();
        itemManager = Manager.GetComponent<ItemManager>();
        battleManager = Manager.GetComponent<BattleManager>();
        npcManager = Manager.GetComponent<NPCManager>();
        settingManager = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingManager>();
        playerPartyManager = this.GetComponent<Party>();
    }
    public void ShowPossibleMovement()
    {
        movementMechanics.WorkOnPossibleMovement(charBase, currentX, currentY);
    }

    public void SetPosition(int x, int y)
    {
        Tile tile = movementMechanics.getCell(x,y);
        if(tile == null)
        {
            return;
        }

        if (tile.isWalkable) // && !tile.isOccupied
        {
            transform.position = tile.transform.position;
            currentX = x;
            currentY = y;

            //Making Old Tile Walkable
            if(currentTileOccupied != null) 
            {
                currentTileOccupied.isWalkable = true;
                beginning = 0; //Local false
            }

            //Remove Previous Highlights & Possiblities
            movementMechanics.clearData();

            //Updating Current Tile
            currentTileOccupied = tile;

            //Making New Tile Non-Walkable
            currentTileOccupied.isWalkable = true;


            //TileBoost
            #region Tile Boost
            if (tile.BoostTile == TileBoost.CP)
            {
                print("CP Full");
                playerPartyManager.Characters[0].CPRefill();
            }
            else if (tile.BoostTile == TileBoost.HP)
            {
                print("HP Full");
                playerPartyManager.Characters[0].HPRefill();
            }
            else if (tile.BoostTile == TileBoost.CharacterAdd)
            {
                playerPartyManager.Characters.Add(tile.BoostCharacter);
                int n = playerPartyManager.Characters.Count;
                playerPartyManager.Characters[n-1].Init();
                tile.BoostTile = TileBoost.None;
            }
            else if (tile.BoostTile == TileBoost.CharacterRemove)
            {
                playerPartyManager.Characters.Remove(playerPartyManager.Characters.Find(x => x.CharBase == tile.BoostCharacter.CharBase));
                int n = playerPartyManager.Characters.Count;
                playerPartyManager.Characters[n-1].Init();
                tile.BoostTile = TileBoost.None;
            }
            #endregion

            StartCoroutine(TileCheckings(tile));

        }
        else
        {
            print(tile.textForBlocked);
        }
    }

    IEnumerator TileCheckings(Tile tile)
    {

        yield return stateManager.GoBusy();

        yield return new WaitForSeconds(beginning);

        //print("Checking Story"); //This is different because progressive blocks can be travelled and still occur story
        yield return storyManager.CheckStory();

        if (!settingManager.travelled.Contains(Position))
        {
            //print("Checking Item");
            yield return itemManager.CheckItem(tile);

            //print("Checking Battle");
            yield return battleManager.CheckBattle(tile);

            settingManager.travelled.Add(Position);
        }
        

        //print("Checking Teleportation");
        yield return TeleCheck(tile);

        yield return stateManager.EnemyTurn();

    }    

    IEnumerator TeleCheck(Tile tile)
    {
        if (tile.isTeleporter && tile.sceneName != "" && !justTeleported)
        {
            yield return stateManager.GoBusy();

            yield return new WaitForSeconds(0.5f);

            stateManager.ChangeScene(tile.sceneName, tile.TeleIndexSpot, tile.xTele, tile.yTele);

        }
        else if (tile.isTeleporter && !justTeleported)
        {
            yield return new WaitForSeconds(0.5f);

            loader.LoadingSpot(tile.spotName);

            movementMechanics.turnOnSpot(tile.TeleIndexSpot); //IDK WHY THIS IS HERE
            justTeleported = true;
            npcManager.WakeUpNPCs();

            SetPosition(tile.xTele, tile.yTele);
            yield return new WaitForSeconds(3f);
            loader.LoadingActive(false);
        }
        else
        {
            justTeleported = false;
        }

    }


}
