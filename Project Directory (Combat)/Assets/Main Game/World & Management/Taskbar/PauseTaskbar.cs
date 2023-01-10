using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseTaskbar : MonoBehaviour
{
    private StateManager stateManager;

    private void Awake()
    {
        stateManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<StateManager>();
    }
    public void TaskBarLoads(string sceneName)
    {
        stateManager?.GoPause();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void ToGame(string currentSceneName)
    {
        stateManager?.GoResume();
        SceneManager.UnloadSceneAsync(currentSceneName);
    }


}
