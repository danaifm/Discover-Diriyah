using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;

public class AccommodationManager : MonoBehaviour
{
    public static AccommodationManager Instance;

    public List<AccommodationRoot> AccommodationsData = new List<AccommodationRoot>();
    public RectTransform ParentTransform;
    public GameObject AccommodationPanel;
    public GameObject UI_Prefab;
    FirebaseFirestore db;
    public FirebaseUser user;
    private CollectionReference fs;
    private QuerySnapshot querySnapshot;
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
    public void GetAccommodationsData()
    {
        AccommodationPanel.SetActive(true);
        Debug.Log("fffff");
        db = FirebaseFirestore.DefaultInstance;
        db.Collection("Accommodation").GetSnapshotAsync().ContinueWithOnMainThread(async task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data: " + task.Exception);
                return;
            }
            AccommodationsData.Clear();
            foreach (var document in task.Result.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();
                foreach (var pair in data)
                {
                    Debug.Log(pair.Key + ": " + pair.Value);
                }
                if (data.ContainsKey("Pictures"))
                {
                    List<object> yourArray = (List<object>)data["Pictures"];
                    foreach (var item in yourArray)
                    {
                        Debug.Log("Image url : " + item.ToString());
                    }
                }
                toggleFav = new toggleFavorite();
                isFav = await toggleFav.isFavorite(document.Id); data.Add("userFavorite", isFav);
                string json = JsonConvert.SerializeObject(data);
                AccommodationRoot AccommodationsRoot = JsonUtility.FromJson<AccommodationRoot>(json);
                AccommodationsData.Add(AccommodationsRoot);
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
        //AccommodationPanel.SetActive(true);
        Debug.Log("DataHandler " + AccommodationsData.Count);
        GameObject temp;
        for (int i = 0; i < AccommodationsData.Count; i++)
        {
            temp = Instantiate(UI_Prefab, ParentTransform);
            temp.GetComponent<AccommodationItem>().Init(AccommodationsData[i]);
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
