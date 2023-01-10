using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrialTest : MonoBehaviour
{
    public void Shift(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
