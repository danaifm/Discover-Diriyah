using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class userStoryTelling : MonoBehaviour
{
    
    public TMP_Text storyText;
    public TMP_Text title;
    public Button nextButton;
    public Button previousButton;
    private string storyPart1;
    private string storyPart2;
    private string storyPart3;
    private int currentPart = 1; 




    FirebaseFirestore db;
    private string storyId = "GLc20E7hGSz7d7qfS6fy";

    void Start()
    {

        db = FirebaseFirestore.DefaultInstance;
        LoadStory();
        InitializeButtonListeners(); // Initialize button listeners

    }

    private void InitializeButtonListeners()
    {
        nextButton.onClick.AddListener(ShowNextPart);
        previousButton.onClick.AddListener(ShowPreviousPart);
        UpdateButtonStates(); // Initialize button states
    }

    private void ShowNextPart()
    {
        if (currentPart < 3) // Ensure there's a next part to show
        {
            currentPart++;
            UpdateStoryDisplay();
            UpdateButtonStates();
        }
    }

    private void ShowPreviousPart()
    {
        if (currentPart > 1) // Ensure there's a previous part to show
        {
            currentPart--;
            UpdateStoryDisplay();
            UpdateButtonStates();
        }
    }

    private void UpdateStoryDisplay()
    {
        switch (currentPart)
        {
            case 1:
                storyText.text = storyPart1;
                break;
            case 2:
                storyText.text = storyPart2;
                break;
            case 3:
                storyText.text = storyPart3;
                break;
        }
    }

    private void UpdateButtonStates()
    {
        nextButton.interactable = currentPart < 3;
        previousButton.interactable = currentPart > 1;

        nextButton.image.color = nextButton.interactable ? GetEnabledColor() : GetDisabledColor();
        previousButton.image.color = previousButton.interactable ? GetEnabledColor() : GetDisabledColor();
    }

    private Color GetEnabledColor()//for button 
    {
        // Convert hex color #824B1E (current button color) to RGB 
        float r = 0x82 / 255f; 
        float g = 0x4B / 255f; 
        float b = 0x1E / 255f; 

        return new Color(r, g, b, 1f); // fully opaque (enabled)
    }
    private Color GetDisabledColor() //for button
    {
        // Convert hex color #824B1E (current button color) to RGB 
        float r = 0x82 / 255f; 
        float g = 0x4B / 255f; 
        float b = 0x1E / 255f; 

        // decrease opacity level
        float decreasedOpacity = 0.8f; 

        // Return the new Color with decreased opacity (dosabled)
        return new Color(r, g, b, decreasedOpacity);
    }



    private void LoadStory()
    {
        DocumentReference docRef = db.Collection("AR Story").Document(storyId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log($"Document data for {snapshot.Id} document:");

                story story = new story();

                story.Title = snapshot.GetValue<string>("Title");
                story.StoryPart1 = snapshot.GetValue<string>("StoryPart1");
                story.StoryPart2 = snapshot.GetValue<string>("StoryPart2");
                story.StoryPart3 = snapshot.GetValue<string>("StoryPart3");
                Debug.Log("finished retreiving story data");
                DisplayStoryData(story);
            }
            else
            {
                Debug.Log($"Document {snapshot.Id} does not exist!");
            }
        });


    }

    


    private void DisplayStoryData(story story)
    {
        title.text = story.Title;
        storyText.text = story.StoryPart1; //display the first part at the beginning
        storyPart1 = story.StoryPart1;
        storyPart2 = story.StoryPart2;
        storyPart3 = story.StoryPart3;

    }
}
