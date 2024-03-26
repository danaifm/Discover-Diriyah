using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryInstance : MonoBehaviour
{
    public static ViewStory instance;
    public static GameObject self;
    private void Awake() {
        self = gameObject;
        DontDestroyOnLoad(gameObject);
    }
    public void SetInstance(ViewStory story)
    {
        instance = story;
        DontDestroyOnLoad(instance);
    }
}
