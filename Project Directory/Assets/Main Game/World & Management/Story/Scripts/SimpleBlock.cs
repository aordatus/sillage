using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Simple Block", menuName = "Assets/Story/Blocks/Simple")]
public class SimpleBlock : Block
{
    [SerializeField] bool repeat = false;
    public override bool Trial(PlayerMovement playerMovement)
    {
        if (repeat && Position == playerMovement.Position) { return true; }

        SettingManager settingManager = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingManager>();
        if (Position == playerMovement.Position && !settingManager.interactedBlocks.Contains(this))
        {
            return true;
        }

        return false;
    }
}
