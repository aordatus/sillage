using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMechanics : MonoBehaviour
{
    private SettingManager currentSetting;
    [SerializeField] private List<Tile> possibleTiles = new List<Tile>();

    private int currentX;
    private int currentY;
    private int range;

    private List<Tile> openList;
    private HashSet<Tile> closedList;

    private bool highlightHelper = true;

    private void Awake()
    {
        currentSetting = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingManager>();
    }
    public void HighlightTiles(bool highlight)
    {
        foreach (Tile t in possibleTiles)
        {
            t.gameObject.transform.GetChild(1).gameObject.SetActive(highlight);
        }
    }

    #region Main (Player)

    public void WorkOnPossibleMovement(CharacterBase charBase, int currentX, int currentY)
    {
        clearData(); //Previous
        this.currentX = currentX;
        this.currentY = currentY;
        range = charBase.MovementRange;

        if (charBase.MovementType == MovementType.PawnUp) { Pawn(0, 1); }
        else if (charBase.MovementType == MovementType.PawnDown) { Pawn(0, -1); }
        else if (charBase.MovementType == MovementType.PawnRight) { Pawn(1, 0); }
        else if (charBase.MovementType == MovementType.PawnLeft) { Pawn(-1, 0); }
        else if (charBase.MovementType == MovementType.PawnHorizontal) { Pawn(1, 0); Pawn(-1, 0); }
        else if (charBase.MovementType == MovementType.PawnVertical) { Pawn(0, 1); Pawn(0, -1); }
        else if (charBase.MovementType == MovementType.Rook) { Pawn(0, 1); Pawn(0, -1); Pawn(1, 0); Pawn(-1, 0); }

        else if (charBase.MovementType == MovementType.BishopUp) { Bishop(1); }
        else if (charBase.MovementType == MovementType.BishopDown) { Bishop(-1); }
        else if (charBase.MovementType == MovementType.Bishop) { Bishop(1); Bishop(-1); }

        else if (charBase.MovementType == MovementType.Queen) { Bishop(1); Bishop(-1); Pawn(0, 1); Pawn(0, -1); Pawn(1, 0); Pawn(-1, 0); }
        else if (charBase.MovementType == MovementType.Horse) { Horse(); }


        if (possibleTiles != null && highlightHelper)
        {
            HighlightTiles(true);
        }
    }

    #endregion

    #region Pathfinding 
    public List<Tile> FindPath(Tile startTile, Tile endTile, CharacterBase charBase)
    {
        highlightHelper = true;
        openList = new List<Tile> { startTile };
        closedList = new HashSet<Tile> { };

        foreach (Tile t in currentSetting.getActiveSpotManager().AllTiles)
        {
            t.gCost = int.MaxValue;
            t.CalculateFCost();
            t.cameFromTile = null;
        }

        startTile.gCost = 0;
        startTile.hCost = CalculateDistanceCost(startTile, endTile);
        startTile.CalculateFCost();

        while (openList.Count > 0)
        {

            Tile currentTile = GetTheLowestFCostTile(openList);

            if(currentTile == endTile)
            {
                    return CalculatePath(endTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            possibleTiles = new List<Tile> { };

            WorkOnPossibleMovement(charBase, currentTile.Position.Item2, currentTile.Position.Item3);
            
            highlightHelper = false;
            foreach (Tile neighbourTile in possibleTiles)
            {
                if (closedList.Contains(neighbourTile))
                {
                    continue;
                }

                int tentativeGCost = currentTile.gCost + CalculateDistanceCost(currentTile, neighbourTile);

                if(tentativeGCost < neighbourTile.gCost)
                {
                    neighbourTile.cameFromTile = currentTile;
                    neighbourTile.gCost = tentativeGCost;
                    neighbourTile.hCost = CalculateDistanceCost(neighbourTile, endTile);
                    neighbourTile.CalculateFCost();

                    if (!openList.Contains(neighbourTile)){
                        openList.Add(neighbourTile);
                    }   
                }

                
 
            }
        }

        //Out of Tiles
        return null;

    }

    private List<Tile> CalculatePath(Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;
        while(currentTile.cameFromTile != null)
        {
            path.Add(currentTile.cameFromTile);
            currentTile = currentTile.cameFromTile;
        }
        path.Reverse();
        return path;
    }


    private int CalculateDistanceCost(Tile a, Tile b)
    {
        int xDistance = Mathf.Abs(a.Position.Item2 - b.Position.Item2);
        int yDistance = Mathf.Abs(a.Position.Item3 - b.Position.Item3);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return 10*Mathf.Min(xDistance, yDistance) + 10*remaining;
            
    }

    private Tile GetTheLowestFCostTile(List<Tile> pathTileList)
    {
        Tile lowestFCostTile = pathTileList[0];
        for (int i = 0; i < pathTileList.Count; i++)
        {
            if (pathTileList[i].fCost < lowestFCostTile.fCost)
            {
                lowestFCostTile = pathTileList[i];
            }
        }

        return lowestFCostTile;

    }

    #endregion

    #region Tile Adders Methods
    private void Pawn(int directionX, int directionY)        //-1 down -1 left
    {
        for (int i = 1; i < range+1; i++)
        {
            (int, int) pos;
            pos.Item1 = currentX + (i * directionX);
            pos.Item2 = currentY + (i * directionY);

            if (posToTile(pos) != null && posToTile(pos).isWalkable) { possibleTiles.Add(posToTile(pos)); } else { break; }
        }
    }

    private void Bishop(int directionY)        //-1 down -1 left
    {
        for (int i = 1; i < range + 1; i++)
        {
            (int, int) pos;
            pos.Item1 = currentX + i;
            pos.Item2 = currentY + (i * directionY);

            if (posToTile(pos) != null && posToTile(pos).isWalkable) { possibleTiles.Add(posToTile(pos)); } else { break; }
        }
        for (int i = 1; i < range + 1; i++)
        {
            (int, int) pos;
            pos.Item1 = currentX - i;
            pos.Item2 = currentY + (i * directionY);

            if (posToTile(pos) != null && posToTile(pos).isWalkable) { possibleTiles.Add(posToTile(pos)); } else { break; }
        }
    }

    private void Horse()        //-1 down -1 left
    {
        int a = 1;
        for (int i = 0; i < 2; i++)
        {
            (int, int) pos;
            pos.Item1 = currentX + 2;
            pos.Item2 = currentY + a;

            if (posToTile(pos) != null && posToTile(pos).isWalkable) { possibleTiles.Add(posToTile(pos)); }
            a *= -1;
        }
        for (int i = 0; i < 2; i++)
        {
            (int, int) pos;
            pos.Item1 = currentX + a;
            pos.Item2 = currentY + 2;

            if (posToTile(pos) != null && posToTile(pos).isWalkable) { possibleTiles.Add(posToTile(pos)); }
            a *= -1;
        }
        for (int i = 0; i < 2; i++)
        {
            (int, int) pos;
            pos.Item1 = currentX - 2;
            pos.Item2 = currentY + a;

            if (posToTile(pos) != null && posToTile(pos).isWalkable) { possibleTiles.Add(posToTile(pos)); }
            a *= -1;
        }
        for (int i = 0; i < 2; i++)
        {
            (int, int) pos;
            pos.Item1 = currentX + a;
            pos.Item2 = currentY - 2;

            if (posToTile(pos) != null && posToTile(pos).isWalkable) { possibleTiles.Add(posToTile(pos)); }
            a *= -1;
        }


    }

    #endregion

    #region Others
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
    private Tile posToTile((int, int) pos)
    {
        Tile tile = getCell(pos.Item1, pos.Item2);
        return tile;
    }

    public Tile getCell(int x, int y)
    {
        return currentSetting.getActiveSpotManager().GetOneTile((currentSetting.getActiveSpotManagerIndex(), x, y));
    }
    public void clearData()
    {
        HighlightTiles(false);
        possibleTiles.Clear();
    }

    public void turnOnSpot(int index)
    {
        currentSetting.turningOnSpot(index);
    }

    public void HighLightOffFull(bool off)
    {
       foreach (Tile t in currentSetting.getActiveSpotManager().AllTiles)
        {
            t.gameObject.transform.GetChild(1).gameObject.SetActive(!off);
        }
    } 
    #endregion
}
