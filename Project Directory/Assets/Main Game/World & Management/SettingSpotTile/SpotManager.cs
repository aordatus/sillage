using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//[ExecuteInEditMode]
public class SpotManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;

    List<Tile> allTiles = new List<Tile>();

    public List<Tile> AllTiles { get { return allTiles; } }

    private void Awake()
    {
        //GenerateMap();
        GetAllTiles();
    }

    private void GetAllTiles()
    {
        foreach(Transform child in transform)
        {
            allTiles.Add(child.gameObject.GetComponent<Tile>());

        }
        print(allTiles.Count());
    }

    public Tile GetOneTile((int indexSpot, int x, int y) pos)
    {
        return allTiles.Find(tile => tile.Position == pos);
    }

    public void GenerateMap()
    {
        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                Tile spawnedTilePrefab = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity, this.transform);
                spawnedTilePrefab.name = $"Tile[x:{x},y:{y}]";
                spawnedTilePrefab.Init(x, y);
            }

        }
    }
}
