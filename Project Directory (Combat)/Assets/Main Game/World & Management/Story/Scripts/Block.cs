using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private int indexSpot;
    [SerializeField] private int x;
    [SerializeField] private int y;

    public string Id { get { return id; } }
    public int IndexSpot { get { return indexSpot; } }
    public (int, int, int) Position { get { return (indexSpot, x, y); } }

    public virtual bool Trial(PlayerMovement playerMovement)
    {
        return false;
    }


}
