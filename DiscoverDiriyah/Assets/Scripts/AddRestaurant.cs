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

public class AddRestaurant : MonoBehaviour
{
    /*
    public TMP_InputField title;
    public TMP_Text titleError;
    public TMP_Text titleCounter;
    public TMP_InputField question;
    public TMP_Text questionError;
    public TMP_Text questionCounter;
    public TMP_InputField correctAnswer;
    public TMP_Text correctAnswerError;
    public TMP_Text correctAnswerCounter;
    //public TMP_InputField CAnswer;
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
    public UnityEvent onCompleteAddMission;
    // Start is called before the first frame update
    void Start()
    {
        alertDialog = FindObjectOfType<AlertDialog>();
        db = FirebaseFirestore.DefaultInstance;
      
        title.characterLimit = 25;
        question.characterLimit = 25;
        correctAnswer.characterLimit = 25;
        wrongAnswer1.characterLimit = 25;
        wrongAnswer2.characterLimit = 25;

    }

    private void Update()
    {
        title.text = title.text.Length + "/" + title.characterLimit;
        question.text = cuisineType.text.Length + "/" + cuisineType.characterLimit;
        locationCounter.text = location.text.Length + "/" + location.characterLimit;
    }
    public void Validation()
    {
        isValid = true;
        ValidateInput(name, nameError);
        string urlPattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
        ValidateLocation(location, locationError, urlPattern);

        string pattern3 = @"^[a-zA-Z ]*$";
        ValidateInput(cuisineType, cuisineError, pattern3);
        
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

  public void ValidateLocation(TMP_InputField inputField, TMP_Text errorText, string pattern = null)
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
            Debug.LogError(inputField.name + " Invalid location URL");
            errorText.text = "Invalid location URL";
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
        Validation();
        uploadEvent();
    }
    public async Task uploadEvent()
    {

        var newRestaurant = new Dictionary<string, object>
        {
            {"Title", name.text},
            {"Question", location.text},
            {"CorrectAnswer", cuisineType.text},
            {"WrongAnswer1", WAnswer1.text},
            {"WrongAnswer2", WAnswer2.text},
            
        };
        
            // Assuming 'db' is already initialized Firestore instance and ready to use
            var docRef = await db.Collection("AR Mission").AddAsync(newRestaurant);
            Debug.Log($"Restaurant added successfully with ID: {docRef.Id}");
#if UNITY_EDITOR
            PlayerPrefs.SetString("restId", docRef.Id); //-- for testing purpose should remove it.
#endif

            
     }
    */
}
