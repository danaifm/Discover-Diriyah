using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;

public class RestaurantsManager : MonoBehaviour
{
    public static RestaurantsManager Instance;

    public List<RestaurantsRoot> RestaurantsData = new List<RestaurantsRoot>();
    public RectTransform ParentTransform;
    public GameObject RestaurantsPanel;
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
    public void GetRestaurantsData()
    {
        RestaurantsPanel.SetActive(true);
        Debug.Log("fffff");
        db = FirebaseFirestore.DefaultInstance;
        db.Collection("Restaurant").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data: " + task.Exception);
                return;
            }
            foreach (var document in task.Result.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();
                foreach (var pair in data)
                {
                    Debug.Log(pair.Key + ": " + pair.Value);
                }
                if (data.ContainsKey("Picture"))
                {
                    List<object> yourArray = (List<object>)data["Picture"];
                    foreach (var item in yourArray)
                    {
                        Debug.Log("Image url : " + item.ToString());
                    }
                }
                string json = JsonConvert.SerializeObject(data);
                RestaurantsRoot EventsRoot = JsonUtility.FromJson<RestaurantsRoot>(json);
                RestaurantsData.Add(EventsRoot);
                // Log the JSON string
                Debug.Log("JSON data: " + json);
                //documentsList.Add(data);
                Debug.Log("/////////////////////////////////");
            }
            DataHandler();
        });
    }
    private void DataHandler()
    {
        //RestaurantsPanel.SetActive(true);
        Debug.Log("DataHandler " + RestaurantsData.Count);
        GameObject temp;
        for (int i = 0; i < 2; i++)
        {
            temp = Instantiate(UI_Prefab, ParentTransform);
            temp.GetComponent<RestaurantsItem>().Init(RestaurantsData[i]);
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
