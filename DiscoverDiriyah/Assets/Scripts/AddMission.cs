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

    //for validation
    string Title;
    string Question;
    string CorrectAnswer;
    string WrongAnswer1;
    string WrongAnswer2;
    
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
         bool isValid = true;

        //TITLE FIELD VALIDATION
        Title = title.text.Trim();
        if (string.IsNullOrWhiteSpace(Title))
        {
            titleError.text = "This field cannot be empty";
            titleError.color = Color.red;
            titleError.fontSize = 3;
            isValid = false;
        }
        else
        {
            titleError.text = "";
        }

        //QUESTION FIELD VALIDATION
        Question = question.text.Trim();
        if (string.IsNullOrWhiteSpace(Question))
        {
            questionError.text = "This field cannot be empty";
            questionError.color = Color.red;
            questionError.fontSize = 3;
            isValid = false;
        }
        else
        {
            questionError.text = "";
        }

        //CorrectAnswer FIELD VALIDATION
        CorrectAnswer = correctAnswer.text.Trim();
        if (string.IsNullOrWhiteSpace(CorrectAnswer))
        {
            correctAnswerError.text = "This field cannot be empty";
            correctAnswerError.color = Color.red;
            correctAnswerError.fontSize = 3;
            isValid = false;
        }
        else
        {
            correctAnswerError.text = "";
        }

        //wrongAnswer1 FIELD VALIDATION
        WrongAnswer1 = wrongAnswer1.text.Trim();
        if (string.IsNullOrWhiteSpace(WrongAnswer1))
        {
            wrongAnswer1Error.text = "This field cannot be empty";
            wrongAnswer1Error.color = Color.red;
            wrongAnswer1Error.fontSize = 3;
            isValid = false;
        }
        else
        {
            wrongAnswer1Error.text = "";
        }

        //wrongAnswer2 FIELD VALIDATION
        WrongAnswer2 = wrongAnswer2.text.Trim();
        if (string.IsNullOrWhiteSpace(WrongAnswer2))
        {
            wrongAnswer2Error.text = "This field cannot be empty";
            wrongAnswer2Error.color = Color.red;
            wrongAnswer2Error.fontSize = 3;
            isValid = false;
        }
        else
        {
            wrongAnswer2Error.text = "";
        }

        //if everything is valid -> upload to firebase 
        if (isValid)
        {
            uploadMission();
        }
/*
        //STAR RATING FIELD VALIDATION
        rating = StarRating.text.Trim();
        string ratingPattern = @"^(5(\.0)?|[0-4](\.\d)?)$";

        if (!Regex.IsMatch(rating, ratingPattern))
        {
            starRatingError.text = "Invalid Star Rating";
            starRatingError.color = Color.red;
            starRatingError.fontSize = 30;
            isValid = false;

        }

        else if (string.IsNullOrWhiteSpace(rating))
        {
            starRatingError.text = "This field cannot be empty";
            starRatingError.color = Color.red;
            starRatingError.fontSize = 30;
            isValid = false;

        }
        else
        {
            starRatingError.text = "";
            starRating = double.Parse(rating);
        }

        //LOCATION FIELD VALIDATION
        location = Location.text.Trim();
        string urlPattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";

        if (!Regex.IsMatch(location, urlPattern))
        {
            locationError.text = "Invalid location URL";
            locationError.color = Color.red;
            locationError.fontSize = 30;
            isValid = false;

        }
        else if (string.IsNullOrWhiteSpace(location))
        {
            locationError.text = "This field cannot be empty";
            locationError.color = Color.red;
            locationError.fontSize = 30;
            isValid = false;

        }
        else
        {
            locationError.text = "";
        }*/
   
    }//end of validation
    

    public void SubmitButtonClick()
    {
        Validation();
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
            Debug.Log($"Mission added successfully with ID: {docRef.Id}");
#if UNITY_EDITOR
            PlayerPrefs.SetString("MissionId", docRef.Id); //-- for testing purpose should remove it.
#endif
            alertDialog.ShowAlertDialog("Mission details added successfully.");
            
     }
    
}
