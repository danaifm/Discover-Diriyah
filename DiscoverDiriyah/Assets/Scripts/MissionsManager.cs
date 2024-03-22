using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class MissionsManager : MonoBehaviour
{
    public List<GameObject> missions = new List<GameObject>();
    public GameObject missionPrefab;
    FirebaseFirestore db;
    QuerySnapshot snapshot;
    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        LoadMissions();
    }

    public void LoadMissions()
    {
        CollectionReference collectionRef = db.Collection("AR Mission");
        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled) {
                Debug.LogError($"Error fetching documents: {task.Exception}");
                return;
            }

            snapshot = task.Result;
            int i = 0;
            foreach (DocumentSnapshot document in snapshot.Documents) {
                Debug.Log($"Document ID: {document.Id}");
                var mission = Instantiate(missionPrefab);
                mission.transform.position = new Vector3(0, mission.transform.position.y - 430*i, 0);
                mission.transform.SetParent(transform);
                missions.Add(mission);
                if (document.Exists)
                {
                    // Map data from the snapshot to the Restaurant object
                    string title = "Title: " + document.GetValue<string>("Title");
                    string question = "Question: " + document.GetValue<string>("Question");
                    string correctAnswer = "CorrectAnswer: " + document.GetValue<string>("CorrectAnswer");
                    string wrongAnswer1 = "WrongAnswer1: " + document.GetValue<string>("WrongAnswer1");
                    string wrongAnswer2 = "WrongAnser2: " + document.GetValue<string>("WrongAnswer2");
                    mission.GetComponent<ViewMissions>().SetTexts(title, question, correctAnswer, wrongAnswer1, wrongAnswer2);
                    Debug.Log($"initialized {i+1}");
                }
                else
                {
                    Debug.Log($"Document {document.Id} does not exist!");
                }
            }
        });

    }
}
