using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ItemTile
{
    [SerializeField] private int indexSpot;
    [SerializeField] private int posX;
    [SerializeField] private int posY;
    [SerializeField] private List<ItemObject> items = new List<ItemObject>();
    public (int, int, int) Position { get { return (indexSpot, posX, posY); } }
    public List<ItemObject> Items { get { return items; } }

}
public class ItemManager : MonoBehaviour
{
    [SerializeField] List<ItemTile> ItemTilesDatabase = new List<ItemTile>();
    private List<ItemObject> itemsFound = new List<ItemObject>();
    [Header("Display")]
    [SerializeField] private GameObject ItemDisplayBox;
    [SerializeField] private Image ItemShowcaseSlot;
    [SerializeField] private TextMeshProUGUI itemText;
    private bool isOpen;

    public IEnumerator CheckItem(Tile tile)
    {
        isOpen = false;

        foreach (ItemTile it in ItemTilesDatabase)
        {
            if (it.Position == tile.Position)
            {
                itemsFound.AddRange(it.Items);
            }
        }
        
        UpdateItemsDisplay();

        while (isOpen)
        {
            yield return null;
        }
       
    }

    public void UpdateItemsDisplay()
    {
        if (itemsFound.Count > 0)
        {
            isOpen = true;
            ItemDisplayBox.SetActive(true);
            ItemShowcaseSlot.sprite = itemsFound[0].Sprite;
            itemText.text = itemsFound[0].Name;

        }
        else
        {
            isOpen = false;
            ItemDisplayBox.SetActive(false);
        }

    }

    public void Throw()
    {
        itemsFound.RemoveAt(0);
        UpdateItemsDisplay();

        
    }

    public void Keep()
    {
        if(PlayerData.ownedItems.Count < PlayerData.capacity)
        {
            PlayerData.ownedItems.Add(itemsFound[0]);
            itemsFound.RemoveAt(0);
            UpdateItemsDisplay();
        }
        else
        {
            print("No ye can't");

        }
                
        
    }
}
