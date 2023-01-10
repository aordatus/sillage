using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("Character")]
    public Character character;
    public Tile currentTileOccupied;
    public NPCType npcType;
    public Tile elementalTile;
    private CharacterBase charBase;
    private (int, int, int) Position;
    private MovementMechanics movementMechanics;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        movementMechanics = this.gameObject.GetComponent<MovementMechanics>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    public void Init()
    {
        transform.position = currentTileOccupied.gameObject.transform.position;
        charBase = character.CharBase;
        Position = currentTileOccupied.Position;
        GetComponent<SpriteRenderer>().sprite = charBase.IconImage;
        SetPosition(currentTileOccupied.Position.Item2, currentTileOccupied.Position.Item3);
        currentTileOccupied.isWalkable = false;

        if(npcType == NPCType.Water)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.35f, 0.35f, 0.35f);
        }
    }

    public void SetPosition(int x, int y)
    {
        Tile tile = movementMechanics.getCell(x, y);
        if (tile == null)
        {
            return;
        }

        if (tile.isWalkable)
        {
            transform.position = tile.transform.position;

            //Remove Previous Highlights & Possiblities
            movementMechanics.clearData();

            //Updating Current Tile
            currentTileOccupied = tile;

            //Record Keeping?


            //Making New Tile Non-Walkable 
            currentTileOccupied.isWalkable = false;

        }
        else
        {
            print(tile.textForBlocked);
        }
    }

    public IEnumerator TryToDoSomething()
    {
        if(npcType == NPCType.Thunder)
        {
            List<Tile> pathCalculated = movementMechanics.FindPath(currentTileOccupied, playerMovement.CurrentTile, charBase);

            movementMechanics.HighLightOffFull(true);

            yield return new WaitForSeconds(0.5f);

            if (pathCalculated != null)
            {
                SetPosition(pathCalculated[1].Position.Item2, pathCalculated[1].Position.Item3);
            }

            else
            {
                print("nope");
            }

        }

        else if(npcType == NPCType.Water)
        {
            if (playerMovement.CurrentTile == elementalTile)
            {
                GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(0.35f, 0.35f, 0.35f);
            }
        }

        else if (npcType == NPCType.Fire)
        {
            if (playerMovement.CurrentTile == elementalTile)
            {
                yield return deletePlayer();
            }
        }
    }

    IEnumerator deletePlayer()
    {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
        currentTileOccupied.isWalkable = true;
    }

    private bool checkIfNearby((int, int, int) Npc, (int, int, int) Pc)
    {
        List<(int, int, int)> PositionsNearby = new List<(int, int, int)>()
        {
            (Pc.Item1+1, Pc.Item2, Pc.Item3),
            (Pc.Item1+1, Pc.Item2-1, Pc.Item3),
            (Pc.Item1+1, Pc.Item2+1, Pc.Item3),
            (Pc.Item1-1, Pc.Item2, Pc.Item3),
            (Pc.Item1-1, Pc.Item2-1, Pc.Item3),
            (Pc.Item1-1, Pc.Item2+1, Pc.Item3),
            (Pc.Item1, Pc.Item2+1, Pc.Item3),
            (Pc.Item1, Pc.Item2-1, Pc.Item3),
        };

        if (PositionsNearby.Contains(Npc))
        {
            return true;
        }
        return false;

    }
}
