using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block", menuName = "Assets/Story/Blocks/ItemNeed")]
public class ItemNeedBlock : Block
{
    [SerializeField] private ItemObject item;
    [SerializeField] private bool reversal; //true means the item should be absent for return true
    [SerializeField] private int itemIndexTile;
    [SerializeField] private int itemXTile;
    [SerializeField] private int itemYTile;
    public override bool Trial(PlayerMovement pm)
    {
        SettingManager settingManager = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingManager>();

        if(!settingManager.travelled.Contains((itemIndexTile, itemXTile, itemYTile)))
        {
            return false;
        }

        if (Position == pm.Position && PlayerData.ownedItems.Contains(item) && !settingManager.interactedBlocks.Contains(this) && !reversal)
        {
            return true;
        }
        else if (Position == pm.Position && !PlayerData.ownedItems.Contains(item) && !settingManager.interactedBlocks.Contains(this) && reversal)
        {
            return true;
        }

        return false;
    }
}
