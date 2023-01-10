using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Animator homeAnimator;
    [SerializeField] private AudioSource menuSong;
    [SerializeField] private GameObject GUI;
    [SerializeField] private Button tapToPlay;
    [SerializeField] private string nextScene;


    void Awake()
    {
        DontDestroyOnLoad(menuSong);
        homeAnimator.enabled = true;
        GUI.SetActive(false);
        StartCoroutine(Start());
        tapToPlay.onClick.AddListener(() => StartCoroutine(End()));
    }

    private IEnumerator End()
    {
        homeAnimator.SetBool("Ending", true);
        GUI.SetActive(false);
        yield return new WaitForSeconds(1f);
        homeAnimator.enabled = false;
        SceneManager.LoadScene(nextScene);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        menuSong.Play();
        GUI.SetActive(true);
    }
}
