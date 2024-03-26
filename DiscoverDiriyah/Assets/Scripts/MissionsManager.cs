using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using System;
using System.Globalization;
using TMPro;

public class MissionsManager : MonoBehaviour
{
    public TMP_Text titleText;
    
    public TMP_Text questionText;
    public TMP_Text correctAnswerText;
    public TMP_Text wrongAnswer1Text;
    public TMP_Text wrongAnswer2Text;
 
    private void Start()
    {
        LoadStories();
    }

    public void LoadStories()
    {
        var missionData = MissionInstance.instance;
        
        titleText.text = missionData.title;
        questionText.text = "Question:\n" + missionData.question;
        correctAnswerText.text = "Correct Answer:\n" + missionData.correctAnswer;
        wrongAnswer1Text.text = "Wrong Answer 1:\n" + missionData.wrongAnswer1;
        wrongAnswer2Text.text = "Wrong Answer 2:\n" + missionData.wrongAnswer2;
    }

    public void ClearInstance()
    {
        Destroy(MissionInstance.instance);
        Destroy(MissionInstance.self);
    }
}
