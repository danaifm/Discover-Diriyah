using Firebase.Firestore;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class AddMission : MonoBehaviour
{
    
    public TMP_InputField title;
    public TMP_Text titleError;
    public TMP_Text titleCounter;
    public TMP_InputField question;
    public TMP_Text questionError;
    public TMP_Text questionCounter;
    public TMP_InputField correctAnswer;
    public TMP_Text correctAnswerError;
    public TMP_Text correctAnswerCounter;
    public TMP_InputField wrongAnswer1;
    public TMP_Text wrongAnswer1Error;
    public TMP_Text wrongAnswer1Counter;
    public TMP_InputField wrongAnswer2;
    public TMP_Text wrongAnswer2Error;
    public TMP_Text wrongAnswer2Counter;
    
    // UnityEvent to be invoked on button click
    FirebaseFirestore db;
    
    Dictionary<string, object> Mission;
    
    bool isValid = true;
    public AlertDialog alertDialog;
    public UnityEvent onCompleteAddEvent;
    

    // Start is called before the first frame update
    void Start()
    {
        alertDialog = FindObjectOfType<AlertDialog>();
        db = FirebaseFirestore.DefaultInstance;
      
        title.characterLimit = 30;
        question.characterLimit = 40;
        correctAnswer.characterLimit = 20;
        wrongAnswer1.characterLimit = 20;
        wrongAnswer2.characterLimit = 20;

    }

    private void Update()
    {
        titleCounter.text = title.text.Length + "/" + title.characterLimit;
        questionCounter.text = question.text.Length + "/" + question.characterLimit;
        correctAnswerCounter.text = correctAnswer.text.Length + "/" + correctAnswer.characterLimit;
        wrongAnswer1Counter.text = wrongAnswer1.text.Length + "/" + wrongAnswer1.characterLimit;
        wrongAnswer2Counter.text = wrongAnswer2.text.Length + "/" + wrongAnswer2.characterLimit;
    }
    public void Validation()
    {
        isValid = true;
        ValidateInput(title, titleError);
        
        string pattern3 = @"^[a-zA-Z ]*$";
        ValidateInput(question, questionError, pattern3);
        
    }
    
    public void ValidateInput(TMP_InputField inputField, TMP_Text errorText, string pattern = null)
    {
        int x = 0;
        if (string.IsNullOrEmpty(inputField.text) || inputField.text.Trim() == "")
        {
            Debug.LogError(inputField.name + " is empty");
            errorText.text = "This field cannot be empty.";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            inputField.image.color = Color.red;
            x = 1;
            isValid = false;
        }else if (pattern != null && !Regex.IsMatch(inputField.text, pattern))
        {
            Debug.LogError(inputField.name + " only string allwed");
            errorText.text = "Only string allowed";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            inputField.image.color = Color.red;
            x = 1;
            isValid = false;
        }
        else
        {
            errorText.text = "";
            inputField.image.color = Color.gray;
        }
    }

    public void SubmitButtonClick()
    {
        //Validation();
        uploadMission();
    }
    public async Task uploadMission()
    {

        var newMission = new Dictionary<string, object>
        {
            {"Title", title.text},
            {"Question", question.text},
            {"CorrectAnswer", correctAnswer.text},
            {"WrongAnswer1", wrongAnswer1.text},
            {"WrongAnswer2", wrongAnswer2.text},
            
        };
        
            // Assuming 'db' is already initialized Firestore instance and ready to use
            var docRef = await db.Collection("AR Mission").AddAsync(newMission);
            Debug.Log($"Restaurant added successfully with ID: {docRef.Id}");
#if UNITY_EDITOR
            PlayerPrefs.SetString("MissionId", docRef.Id); //-- for testing purpose should remove it.
#endif
            alertDialog.ShowAlertDialog("Mission details added successfully.");
            
     }
    
}
