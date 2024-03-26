using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewMissions : MonoBehaviour
{
    public TMP_Text titleText;
    public string title;
    public string question;
    public string correctAnswer;
    public string wrongAnswer1;
    public string wrongAnswer2;
    public MissionInstance missionInstance;

    private void Start() {
        missionInstance = FindObjectOfType<MissionInstance>();
    }

    public void SetTitle(string title)
    {
        titleText.text = title;
        this.title = title;
        Debug.Log(title);
    }

    public void SaveTexts(string title, string question, string correctAnswer, string wrongAnswer1, string wrongAnswer2)
    {
        this.title = title;
        this.question = question;
        this.correctAnswer = correctAnswer;
        this.wrongAnswer1 = wrongAnswer1;
        this.wrongAnswer2 = wrongAnswer2;
    }

    public void LoadMission()
    {
        missionInstance.SetInstance(this);
        Debug.Log("Loading..");
        SceneManager.LoadScene("Mission");
    }
}
