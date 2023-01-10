using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBoxBase : MonoBehaviour
{
    private LogManager logManager; 

    private void Awake()
    {
        logManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<LogManager>();

        this.GetComponent<Button>().onClick.AddListener(delegate { logManager.OnLogBoxClick(this.gameObject); });
    }


}
