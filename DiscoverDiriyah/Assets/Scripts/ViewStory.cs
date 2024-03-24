using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class ViewStory : MonoBehaviour
{
    public static ViewStory instance;
    public TMP_Text titleText;
    public string title;
    public string storyPart1;
    public string storyPart2;
    public string storyPart3;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetTitle(string title)
    {
        titleText.text = title;
        this.title = title;
        Debug.Log(title);
    }

    public void SaveTexts(string title, string storyPart1, string storyPart2, string storyPart3)
    {
        this.title = title;
        this.storyPart1 = storyPart1;
        this.storyPart2 = storyPart2;
        this.storyPart3 = storyPart3;
    }

    public void LoadStory()
    {
        Debug.Log("Loading..");
        SceneManager.LoadScene("Story");
    }
}
