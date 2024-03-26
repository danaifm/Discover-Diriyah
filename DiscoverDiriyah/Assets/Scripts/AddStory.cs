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

public class AddStory : MonoBehaviour
{
    
    public TMP_InputField title;
    public TMP_Text titleError;
    public TMP_Text titleCounter;
    public TMP_InputField storyPart1;
    public TMP_Text storyPart1Error;
    public TMP_Text storyPart1Counter;
    public TMP_InputField storyPart2;
    public TMP_Text storyPart2Error;
    public TMP_Text storyPart2Counter;
    public TMP_InputField storyPart3;
    public TMP_Text storyPart3Error;
    public TMP_Text storyPart3Counter;
    
    //for validation
    string Title;
    string StoryPart1;
    string StoryPart2;
    string StoryPart3;
    
    // UnityEvent to be invoked on button click
    FirebaseFirestore db;
    
    Dictionary<string, object> Story;
    
    bool isValid = true;
    public AlertDialog alertDialog;
    public UnityEvent onCompleteAddEvent;
    

    // Start is called before the first frame update
    void Start()
    {
        alertDialog = FindObjectOfType<AlertDialog>();
        db = FirebaseFirestore.DefaultInstance;
      
        title.characterLimit = 30;
        storyPart1.characterLimit = 50;
        storyPart2.characterLimit = 50;
        storyPart3.characterLimit = 50;
    }

    private void Update()
    {
        titleCounter.text = title.text.Length + "/" + title.characterLimit;
        storyPart1Counter.text = storyPart1.text.Length + "/" + storyPart1.characterLimit;
        storyPart2Counter.text = storyPart2.text.Length + "/" + storyPart2.characterLimit;
        storyPart3Counter.text = storyPart3.text.Length + "/" + storyPart3.characterLimit;
    }
    public void Validation()
    {
        isValid = true;
        ValidateInput(title, titleError);
        
        string pattern3 = @"^[a-zA-Z ]*$";
        ValidateInput(title, titleError, pattern3);
        
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
        uploadStory();
    }
    public async Task uploadStory()
    {

        var newStory = new Dictionary<string, object>
        {
            {"Title", title.text},
            {"StoryPart1", storyPart1.text},
            {"StoryPart2", storyPart2.text},
            {"StoryPart3", storyPart3.text},
        };
        
            // Assuming 'db' is already initialized Firestore instance and ready to use
            var docRef = await db.Collection("AR Story").AddAsync(newStory);
            Debug.Log($"Story is added successfully with ID: {docRef.Id}");
#if UNITY_EDITOR
            PlayerPrefs.SetString("StoryId", docRef.Id); //-- for testing purpose should remove it.
#endif
            alertDialog.ShowAlertDialog("Story details added successfully.");
            
     }
    
}
