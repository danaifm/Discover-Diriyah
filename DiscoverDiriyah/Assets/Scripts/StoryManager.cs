using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using TMPro;

public class StoryManager : MonoBehaviour
{
    public TMP_Text titleText;
    
    public TMP_Text storyPart1Text;
    public TMP_Text storyPart2Text;
    public TMP_Text storyPart3Text;
 
    private void Start()
    {
        LoadStories();
    }

    public void LoadStories()
    {
        var storyData = ViewStory.instance;
        
        titleText.text = storyData.title;
        storyPart1Text.text = storyData.storyPart1;
        storyPart2Text.text = storyData.storyPart2;
        storyPart3Text.text = storyData.storyPart3;
    }
}
