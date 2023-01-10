using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block", menuName = "Assets/Story/Blocks/Progressive")]
public class ProgressiveBlock : Block
{
    [SerializeField] private int needIndexTile;
    [SerializeField] private int needXTile;
    [SerializeField] private int needYTile;

    public override bool Trial(PlayerMovement pm)
    {
        SettingManager settingManager = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingManager>();
        if (Position == pm.Position && settingManager.travelled.Contains((needIndexTile, needXTile, needYTile)) && !settingManager.interactedBlocks.Contains(this))
        {
            return true;
        }

        return false;
    }
}
