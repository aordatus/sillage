using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject prefabItemSlot;
    [SerializeField] private TextMeshProUGUI textSlot;
    [SerializeField] private Button[] itemButtons;

    private void Awake()
    {
         UpdatingInventory();
    }

    private void UpdatingInventory()
    {
        ClearingInventory();
        StartingInventory();
    }

    private void StartingInventory()
    {
        for (int i = 0; i < PlayerData.capacity; i++)
        {
            ItemSlot instantiatedSlot = Instantiate(prefabItemSlot, scrollContent).GetComponent<ItemSlot>();
            try
            {
                instantiatedSlot.itemObject = PlayerData.ownedItems[i];
                instantiatedSlot.Init();
            }
            catch { }
            
        }
    }
    private void ClearingInventory()
    {
        foreach(Transform t in scrollContent)
        {
            Destroy(t.gameObject);
        }
        ClearDisplay();
    }
    public void DisplayItemInfo(ItemObject itemObject)
    {
        ClearDisplay();

        if (itemObject != null)
        {
            textSlot.text = itemObject.Name + "\n" + itemObject.Description;
            ButtonsSummon(itemObject);
        }

        else
        {
            textSlot.text = "Slot is Empty.";
        }
    }

    private void ButtonsSummon(ItemObject itemObject)
    {
        int indexCal = 0;

        if (itemObject.TypeItem == ItemType.CharacterPlus)
        {
            TurnButtonOn(itemButtons[indexCal], "Consumable");
            indexCal += 1;
        }
        if (itemObject.TypeItem == ItemType.CharacterBonus)
        {
            TurnButtonOn(itemButtons[indexCal], "Activate");
            indexCal += 1;
        }
        if (itemObject.Throwable)
        {
            TurnButtonOn(itemButtons[indexCal], "Throw");
        }   
    }

    private void ClearDisplay()
    {
        foreach (Button b in itemButtons)
        {
            b.gameObject.SetActive(false);
        }
        textSlot.text = "";
    }

    private void TurnButtonOn(Button B, string text)
    {
        B.gameObject.SetActive(true);
        B.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }
}

