using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance;

    public List<EventRoot> EventsData = new List<EventRoot>();
    public RectTransform ParentTransform;
    public GameObject EventsPanel;
    public GameObject UI_Prefab;
    FirebaseFirestore db;

   

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {

    }
    public void GetEventsData()
    {
        Debug.Log("fffff");
        EventsPanel.SetActive(true);
        db = FirebaseFirestore.DefaultInstance;
        db.Collection("Event").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data: " + task.Exception);
                return;
            }
            EventsData.Clear();
            foreach (var document in task.Result.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();
                string Start_Date = "";
                string End_Date = "";
                string timestamp = "";
                foreach (var pair in data)
                {
                    Debug.Log(pair.Key + "|: " + pair.Value);
                    if (pair.Key == "StartDate")
                    {
                        timestamp = pair.Value.ToString();
                        timestamp = timestamp.Substring(timestamp.IndexOf(':') + 2);
                        Start_Date = timestamp;
                    }
                    else if (data.ContainsKey("EndDate"))
                    {
                        timestamp = pair.Value.ToString();
                        timestamp = timestamp.Substring(timestamp.IndexOf(':') + 2);
                        End_Date = timestamp;
                    }
                    else
                    {
                        Debug.Log("No date found");
                    }
                }
                if (data.ContainsKey("Pictures"))
                {
                    List<object> yourArray = (List<object>)data["Pictures"];
                    foreach (var item in yourArray)
                    {
                        Debug.Log("Image url : " + item.ToString());
                    }
                }
                
                string json = JsonConvert.SerializeObject(data);
                EventRoot EventsRoot = JsonUtility.FromJson<EventRoot>(json);
                EventsRoot.StartDate = Start_Date;
                EventsRoot.EndDate = End_Date;
                EventsData.Add(EventsRoot);
                Debug.Log("JSON data: " + json);
                Debug.Log("/////////////////////////////////");
            }
            DataHandler();
        });
    }
    private void DataHandler()
    {
        //EventsPanel.SetActive(true);
        Debug.Log("DataHandler " + EventsData.Count);
        GameObject temp;
        for (int i = 0; i < EventsData.Count; i++)
        {
            temp = Instantiate(UI_Prefab, ParentTransform);
            temp.GetComponent<Event_Item>().Init(EventsData[i]);
        }
    }
    public void DiscardData()
    {
        foreach (Transform child in ParentTransform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
    }
   
}
