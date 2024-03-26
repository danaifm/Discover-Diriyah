using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using System;
using System.Globalization;

public class MissionsManager : MonoBehaviour
{
    public List<GameObject> missions = new List<GameObject>();
    public GameObject missionPrefab;
    FirebaseFirestore db;
    QuerySnapshot snapshot;
    public AlertDialog alertDialog;
    public List<DateTime> dateTimes = new List<DateTime>();
    public List<string> dateTimeStrs;
    private void Start()
    {
        alertDialog = FindObjectOfType<AlertDialog>();
        alertDialog.ShowLoading();
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
                    dateTimeStrs.Add(document.GetValue<string>("Time"));
                    mission.GetComponent<ViewMissions>().SetTexts(title, question, correctAnswer, wrongAnswer1, wrongAnswer2);
                    Debug.Log($"initialized {i+1}");
                }
                else
                {
                    Debug.Log($"Document {document.Id} does not exist!");
                }
            }
            StartCoroutine(Sort());
            alertDialog.HideLoading();
        });
    }

    public IEnumerator AddDatTimeCoroutine(DateTime newDateTime)
    {
        AddDateTime(newDateTime);
        yield return null;
    }

    public IEnumerator Sort()
    {
        foreach (var time in dateTimeStrs)
        {
            StartCoroutine(AddDatTimeCoroutine(DateTime.ParseExact(time, "dd-MM-yyyy, HH:mm:ss", CultureInfo.InvariantCulture)));
        }
        foreach (var date in dateTimes)
        {
            Debug.Log(date.ToString("dd-MM-yyyy, HH:mm:ss"));
        }
        Debug.Log("1");
        List<DateTime> parsedDates = dateTimeStrs.ConvertAll(str => DateTime.ParseExact(str, "dd-MM-yyyy, HH:mm:ss", CultureInfo.InvariantCulture));
        Debug.Log("1.1");

        List<int> newIndices = new List<int>();
        Debug.Log("1.2");

        foreach (var date in parsedDates)
        {
            int index = dateTimes.IndexOf(date);
            newIndices.Add(index);
        }
        Debug.Log("2");

        StartCoroutine(ReorderCoroutine(newIndices));
        Debug.Log("Complete");

        yield return null;
    }

    public void AddDateTime(DateTime newDateTime)
    {
        Debug.Log("Date: " + newDateTime.ToString("dd-MM-yyyy, HH:mm:ss")); // Use HH for 24-hour format

        // If the list is empty, or the new date is the newest, add it to the beginning
        if (dateTimes.Count == 0 || newDateTime >= dateTimes[0])
        {
            dateTimes.Insert(0, newDateTime);
            Debug.Log("Inserted at the beginning");
        }
        else
        {
            // The loop now correctly iterates through the list and checks if the new date is newer than the current item
            for (int i = 0; i < dateTimes.Count; i++)
            {
                // If the new date is newer than the date at position 'i', insert the new date before 'i'
                if (newDateTime > dateTimes[i])
                {
                    dateTimes.Insert(i, newDateTime);
                    Debug.Log("Inserted at position " + i);
                    return; // Exit the method after insertion to prevent adding the date again at the end
                }
            }

            // If the loop completes without finding a newer date, the new date is the oldest and should be added at the end
            dateTimes.Add(newDateTime);
            Debug.Log("Inserted at the end");
        }
    }

    public IEnumerator ReorderCoroutine(List<int> newIndices)
    {
        ReorderGameObjects(newIndices);
        yield return null;
    }

    void ReorderGameObjects(List<int> newIndices)
    {
        // Temporary list to hold the new order
        List<Transform> newOrder = new List<Transform>();

        for (int i = 0; i < newIndices.Count; i++)
        {
            // Find the child that needs to be moved to position i
            int newIndex = newIndices.IndexOf(i);
            newOrder.Add(transform.GetChild(newIndex));
        }

        // Disable and then re-parent in the correct order
        foreach (var child in newOrder)
        {
            child.gameObject.SetActive(false);
        }
        foreach (var child in newOrder)
        {
            child.SetParent(null); // Detach from parent
            child.SetParent(transform); // Re-attach to parent
            child.gameObject.SetActive(true);
        }
    }
}
