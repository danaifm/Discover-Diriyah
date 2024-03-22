using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ViewMissions : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text questionText;
    public TMP_Text correctAnswerText;
    public TMP_Text wrongAnswer1Text;
    public TMP_Text wrongAnswer2Text;

    public void SetTexts(string title, string question, string correct, string wrong1, string wrong2)
    {
        titleText.text = title;
        questionText.text = question;
        correctAnswerText.text = correct;
        wrongAnswer1Text.text = wrong1;
        wrongAnswer2Text.text = wrong2;
        Debug.Log(title);
    }
}
