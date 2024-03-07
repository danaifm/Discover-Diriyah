using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;

public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance;

    public List<EventRoot> EventsData = new List<EventRoot>();
    public RectTransform ParentTransform;
    public GameObject EventsPanel;
    public GameObject UI_Prefab;
    FirebaseFirestore db;
    private bool isFav;
    private toggleFavorite toggleFav;

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
        db.Collection("Event").GetSnapshotAsync().ContinueWithOnMainThread(async task =>
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
                        Debug.Log("StartDate timestamp ::" + timestamp);
                        Start_Date = timestamp;
                        //Start_Date = pair.Value.ToString();
                    }
                    if (pair.Key == "EndDate")
                    {
                        timestamp = pair.Value.ToString();
                        timestamp = timestamp.Substring(timestamp.IndexOf(':') + 2);
                        Debug.Log("EndDate timestamp ::" + timestamp);
                        End_Date = timestamp;
                        //End_Date = pair.Value.ToString(); ;
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
                toggleFav = gameObject.AddComponent<toggleFavorite>();
                isFav = await toggleFav.isFavorite(document.Id);
                data.Add("userFavorite", isFav);
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
