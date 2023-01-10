using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> allSpots = new List<GameObject>();
    private GameObject activeSpot;

    [Space(10)]
    [SerializeField] private int startingSpot;
    [SerializeField] private int startingX;
    [SerializeField] private int startingY;

    public (int,int,int) startingPosition { get { return (startingSpot, startingX, startingY); } }
    public List<(int, int, int)> travelled = new List<(int, int, int)>(); //index,posx,posy
    public List<Block> interactedBlocks = new List<Block>();

    [SerializeField] int travelledCount; //NO USE RIGHT NOW
    private void Update()
    {
        travelledCount = travelled.Count;
    }
    public SpotManager getActiveSpotManager()
    {
        if (activeSpot == null)
        {
            turningOnSpot(0);
        }
        return activeSpot.GetComponent<SpotManager>();
    }
    public int getActiveSpotManagerIndex()
    {
        return allSpots.IndexOf(activeSpot);
    }

    public void turningOnSpot(int spotIndex)
    {
        activeSpot = allSpots[spotIndex];
        turningOffOthers();
    }

    private void turningOffOthers()
    {
        foreach(GameObject go in allSpots)
        {
            if(go != activeSpot)
            {
                go.SetActive(false);
            }
            else
            {
                go.SetActive(true);
            }
        }

    }

}
