using Firebase.Firestore;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    string Input;
    bool isValid = true;
    
    // UnityEvent to be invoked on button click
    FirebaseFirestore db;
    
    Dictionary<string, object> Mission;
    
    
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

    public void Validation() {
         isValid = true;
         ValidateInput(title, titleError);
         ValidateInput(question, questionError);
         ValidateInput(correctAnswer, correctAnswerError);
         ValidateInput(wrongAnswer1, wrongAnswer1Error);
         ValidateInput(wrongAnswer2, wrongAnswer2Error);   
    }
    public void ValidateInput(TMP_InputField input, TMP_Text errorText)
    {
        Input = input.text.Trim();
        bool containsSpecialChar = Input.All(c => char.IsLetterOrDigit(c) || "!%^*()-_+=?/.;,:".Contains(c));
        //bool containsLink = Input.Contains("http://") || Input.Contains("https://");
        // List of known protocols
        string[] protocols = { "http://", "https://", "ftp://", "mailto:", "file://" };
        bool containsLink = protocols.Any(p => Input.ToLower().Contains(p.ToLower()));

        if (string.IsNullOrWhiteSpace(Input))
        {
            errorText.text = "This field cannot be empty";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            isValid = false;
        }else if (containsLink){
            errorText.text = "Links are not allowed";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            isValid = false;
        }else if(!containsSpecialChar){
            errorText.text = "Only ,.:;/?!()_-*+^=% are allowed";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            isValid = false;
        }else
        {
            errorText.text = "";
        }
    }//end of validation
    

    public void SubmitButtonClick()
    {
        Debug.Log($"isValid");
        Validation();
        Debug.Log(isValid);
        //if everything is valid -> upload to firebase 
        if (isValid)
        {
            alertDialog.ShowLoading();
            uploadMission();
        }
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
            {"Time", DateTime.Now.ToString("dd-MM-yyyy, HH:mm:ss")}
        };
        
            // Assuming 'db' is already initialized Firestore instance and ready to use
            var docRef = await db.Collection("AR Mission").AddAsync(newMission);
            Debug.Log($"Mission added successfully with ID: {docRef.Id}");
#if UNITY_EDITOR
            PlayerPrefs.SetString("MissionId", docRef.Id); //-- for testing purpose should remove it.
#endif
            alertDialog.HideLoading();
            alertDialog.ShowAlertDialog("Mission details added successfully.");
            
     }
} 
