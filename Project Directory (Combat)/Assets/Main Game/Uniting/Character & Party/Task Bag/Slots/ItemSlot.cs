using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemObject itemObject;

    public void Init()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemObject.Name;
        transform.GetChild(1).GetComponent<Image>().sprite = itemObject.Sprite;
    }

    public string Description()
    {
        return itemObject.Description;
    }

    public void OnClickItemSlot()
    {
        GameObject.FindGameObjectWithTag("InvManager").GetComponent<InventoryManager>().DisplayItemInfo(itemObject);
    }
}
