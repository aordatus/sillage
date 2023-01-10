using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block", menuName = "Assets/Story/Blocks/TileChanger")]
public class TileChangerBlock : Block
{
    [SerializeField] private int changeIndexTile;
    [SerializeField] private int changeXTile;
    [SerializeField] private int changeYTile;
    [SerializeField] private TileType tileType;
    [SerializeField] private bool TileChangeWithItem;
    [SerializeField] private ItemObject item;

    public override bool Trial(PlayerMovement pm)
    {
        
        SettingManager settingManager = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingManager>();
        Tile toChangeTile = settingManager.getActiveSpotManager().GetOneTile((changeIndexTile, changeXTile, changeYTile));

        if (Position == pm.Position && !settingManager.interactedBlocks.Contains(this))
        {
            if(TileChangeWithItem && !PlayerData.ownedItems.Contains(item))
            {
                return false;
            }

            toChangeTile.TypeTile = tileType;
            toChangeTile.ColorFill();
            return true;
        }

        return false;
    }
}
