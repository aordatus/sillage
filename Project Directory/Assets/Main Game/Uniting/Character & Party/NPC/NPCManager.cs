using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType
{
    Rock, //stay but no interaction
    Water, //stay + interact
    Fire, //water but gets deleted
    Air, //Follower
    Thunder //Follow + Fight
}

[System.Serializable]
public class NPCSpot
{
    [SerializeField] private Tile tile;
    [SerializeField] private Character character;
    [SerializeField] private NPCType npcType;
    [SerializeField] private Tile elementalTile;

    public Character Character { get { return character; } }
    public Tile Tile { get { return tile; } }
    public Tile ElementalTile { get { return elementalTile; } }

    public NPCType NpcType { get { return npcType; } }

}
public class NPCManager : MonoBehaviour
{
    public List<NPCSpot> npcSpots = new List<NPCSpot>();
    public Transform npcSummonGameObject;
    public GameObject npcPrefab;
    SettingManager currentSetting;

    public List<NPCMovement> activeNPC = new List<NPCMovement>();

    private void Awake()
    {
        currentSetting = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingManager>();
    }
    public void Start()
    {
        foreach (NPCSpot npcSpot in npcSpots)
        {
            NPCMovement EM = Instantiate(npcPrefab, npcSummonGameObject).GetComponent<NPCMovement>();
            EM.character = npcSpot.Character;
            EM.currentTileOccupied = npcSpot.Tile;
            EM.npcType = npcSpot.NpcType;
            if(npcSpot.ElementalTile != null)
            {
                EM.elementalTile = npcSpot.ElementalTile;
            }
            EM.Init();
        }

        WakeUpNPCs();
    }

    public void WakeUpNPCs()
    {
        int activeIndex = currentSetting.getActiveSpotManagerIndex();
        foreach (Transform t in npcSummonGameObject)
        {
            NPCMovement npcm = t.gameObject.GetComponent<NPCMovement>();
            int npcIndex = npcm.currentTileOccupied.IndexSpot;
            if (npcIndex == activeIndex)
            {
                if (!activeNPC.Contains(npcm))
                {
                    activeNPC.Add(npcm);
                }

                t.gameObject.SetActive(true);
            }
            else
            {
                if (activeNPC.Contains(npcm))
                {
                    activeNPC.Remove(npcm);
                }
                t.gameObject.SetActive(false);
            }

        }
    }




}
