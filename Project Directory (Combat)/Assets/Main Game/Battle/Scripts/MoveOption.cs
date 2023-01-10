using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOption : MonoBehaviour
{
    public Ability ability { get; set; }

    public void OnClickMove()
    {
        BattleManager battleManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<BattleManager>();
        StartCoroutine(battleManager.PlayerClickedOption(ability));
    }
    
}
