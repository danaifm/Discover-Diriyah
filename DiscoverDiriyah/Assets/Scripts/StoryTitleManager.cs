using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class StoryTitleManager : MonoBehaviour
{
    public List<GameObject> stories = new List<GameObject>();
    public GameObject storyPrefab;
    public AlertDialog alertDialog;
    FirebaseFirestore db;
    QuerySnapshot snapshot;
    public GameObject LoadingObject;
    private void Start()
    {
        alertDialog = FindObjectOfType<AlertDialog>();
        alertDialog.ShowLoading();
        db = FirebaseFirestore.DefaultInstance;
        Debug.Log("Loading");
        LoadTitle();
    }

    public void LoadTitle()
    {
        CollectionReference collectionRef = db.Collection("AR Story");

        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled) {
                Debug.LogError($"Error fetching documents: {task.Exception}");
                return;
            }

            snapshot = task.Result;
            int i = 0;
            foreach (DocumentSnapshot document in snapshot.Documents) {
                Debug.Log($"Document ID: {document.Id}");
                var story = Instantiate(storyPrefab);
                story.transform.position = new Vector3(0, story.transform.position.y - 430*i, 0);
                story.transform.SetParent(transform);
                stories.Add(story);
                if (document.Exists)
                {
                    // Map data from the snapshot to the Restaurant object
                    string title = document.GetValue<string>("Title");
                    string storyPart1 = document.GetValue<string>("StoryPart1");
                    string storyPart2 = document.GetValue<string>("StoryPart2");
                    string storyPart3 = document.GetValue<string>("StoryPart3");
                    story.GetComponentInChildren<ViewStory>().SetTitle(title);
                    story.GetComponentInChildren<ViewStory>().SaveTexts(title, storyPart1, storyPart2, storyPart3);
                    Debug.Log($"initialized {i+1}");
                }
                else
                {
                    Debug.Log($"Document {document.Id} does not exist!");
                }
            }
        });
        alertDialog.HideLoading();
    }
}
