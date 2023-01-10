using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Mountain,
    Valley,
    CastleWalls,
    CastlePath
}
public enum TileBoost
{
    None,
    HP,
    CP,
    CharacterAdd,
    CharacterRemove
}

[ExecuteInEditMode]
public class Tile : MonoBehaviour
{
    [SerializeField] TileType tileType;
    public bool isWalkable = true;

    public string textForBlocked = "";
    [SerializeField] private int x;
    [SerializeField] private int y;
    [SerializeField] private int currentIndexSpot;
    [SerializeField] TileBoost boostTile;
    [SerializeField] Character boostCharacter = null;

    public (int, int, int) Position { get { return (currentIndexSpot, x, y); } }
    public int IndexSpot { get { return currentIndexSpot; } }
    public TileType TypeTile { get { return tileType; } set { tileType = value; } }
    public TileBoost BoostTile { get { return boostTile; } set { boostTile = value; } }
    public Character BoostCharacter { get { return boostCharacter; } }

    [Header("Teleporter")]
    public bool isTeleporter;
    public string sceneName;
    public string spotName;
    public int TeleIndexSpot;
    public int xTele;
    public int yTele;

    [Header("Pathfinding")]
    public int gCost;
    public int fCost;
    public int hCost;
    public Tile cameFromTile;


    private PlayerMovement PM;
    private StateManager SM;

    private SpriteRenderer spriteRenderer;
    private GameObject highlight;
    [HideInInspector] public GameObject highlight2;

  
    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    private void Awake()
    {
        
        PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        SM = GameObject.FindGameObjectWithTag("Manager").GetComponent<StateManager>();
        

        spriteRenderer = GetComponent<SpriteRenderer>();
        highlight = transform.GetChild(0).gameObject;
        highlight2 = transform.GetChild(1).gameObject;

        ColorFill();
        //Renaming Fuck Up SomeFix();
    }

    private void SomeFix()
    {
        string a = gameObject.name;
        int xx = int.Parse(Between(a, "x:", ","));
        this.x = xx;
        int yy = int.Parse(Between(a, "y:", "]"));
        this.y = yy;

    }

    private void OnMouseEnter()
    {
        if(!isWalkable && PM.MovingAllowed && highlight2.activeInHierarchy == true && SM.ReadGameState == GameState.playerTurn)
        {
            highlight.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (PM.MovingAllowed && highlight2.activeInHierarchy == true && SM.ReadGameState == GameState.playerTurn)
        {
            PM.SetPosition(x, y);
        }
    }

    public void ColorFill()
    {
        if (tileType == TileType.Mountain)
        {
            spriteRenderer.color = new Color(0.15f, 0.09f, 0.06f, 1f);
            isWalkable = false;
        }

        else if (tileType == TileType.Valley)
        {
            spriteRenderer.color = new Color(0.33f, 0.23f, 0.19f, 1f);
            isWalkable = true;
        }

        else if (tileType == TileType.CastlePath)
        {
            spriteRenderer.color = new Color(0.83f, 0.83f, 0.83f, 1f);
            isWalkable = true;
        }

        else if (tileType == TileType.CastleWalls)
        {
            spriteRenderer.color = new Color(0.23f, 0.23f, 0.23f, 1f);
            isWalkable = false;
        }

    }
    private string Between(string STR, string FirstString, string LastString)
    {
        string FinalString;
        int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
        int Pos2 = STR.IndexOf(LastString);
        FinalString = STR.Substring(Pos1, Pos2 - Pos1);
        return FinalString;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

}

