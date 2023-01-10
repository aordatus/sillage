using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Loader : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] List<string> stringtexts;
    [SerializeField] Image wallImage;
    [SerializeField] Sprite vriwonWall;
    [SerializeField] Sprite easoWall;
    [SerializeField] Slider slider;

    private void Awake()
    {
        LoadingActive(false);
        slider.value = 0;
    }

    public void LoadingActive(bool activeStatus)
    {
        LoadingScreen.SetActive(activeStatus);
    }
    public void LoadingScene(string sceneName, ChapterInBox CIB)
    {
        StartCoroutine(LoadSceneAsynchronously(sceneName, CIB.SettingName, CIB.Title));
    }
    public void LoadingSpot(string spotName)
    {
        slider.value = 0;
        LoadingScreen.SetActive(true);
        text.text = $"Heading to {spotName}. {stringtexts[Random.Range(0, stringtexts.Count)]}";
        StartCoroutine(lolSlider());
    }

    IEnumerator LoadSceneAsynchronously(string sceneName, string settingName, string chapterName)
    {
        yield return null;

        LoadingScreen.SetActive(true);
        text.text = $"Loading {chapterName} and heading to {settingName}. {stringtexts[Random.Range(0, stringtexts.Count)]}";
        if (sceneName.Contains("Easo")) { wallImage.sprite = easoWall; }
        else { wallImage.sprite = vriwonWall; }


        yield return lolSlider();
        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator lolSlider()
    {
        while (slider.value < slider.maxValue)
        {
            slider.value += 0.005f;
            yield return null;
        }
    }


    public void Reloading()
    {
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }
}
