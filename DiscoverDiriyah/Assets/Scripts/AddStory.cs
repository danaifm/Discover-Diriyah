using Firebase.Extensions;
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
    string Input;
    bool isValid = true;
    
    // UnityEvent to be invoked on button click
    FirebaseFirestore db;
    
    Dictionary<string, object> Story;
    
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
         ValidateInput(storyPart1, storyPart1Error);
         ValidateInput(storyPart2, storyPart2Error);
         ValidateInput(storyPart3, storyPart3Error);  
    }
    
   public void ValidateInput(TMP_InputField input, TMP_Text errorText)
    {
        Input = input.text.Trim();
        bool containsSpecialChar = Input.All(c => char.IsLetterOrDigit(c) || "!%^*()-_+=?/.;,:".Contains(c));
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
            uploadStory();
        }
    }

    public async Task uploadStory()
    {
        
        var newStory = new Dictionary<string, object>
        {
            {"Title", title.text},
            {"StoryPart1", storyPart1.text},
            {"StoryPart2", storyPart2.text},
            {"StoryPart3", storyPart3.text},
            {"Time", DateTime.Now.ToString("dd-MM-yyyy, HH:mm:ss")}
        };
        
        Debug.Log(DateTime.Now.ToString("dd-MM-yyyy, HH:mm:ss"));
        // Assuming 'db' is already initialized Firestore instance and ready to use
        var docRef = await db.Collection("AR Story").AddAsync(newStory);
        Debug.Log($"Story is added successfully with ID: {docRef.Id}");
        #if UNITY_EDITOR
        PlayerPrefs.SetString("StoryId", docRef.Id); //-- for testing purpose should remove it.
        #endif
        alertDialog.HideLoading();
        alertDialog.ShowAlertDialog("Story details added successfully.");
            
     }
    
}
