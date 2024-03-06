using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;

public class RestaurantsManager : MonoBehaviour
{
    public static RestaurantsManager Instance;

    public List<RestaurantsRoot> RestaurantsData = new List<RestaurantsRoot>();
    public RectTransform ParentTransform;
    public GameObject RestaurantsPanel;
    public GameObject UI_Prefab;
    FirebaseFirestore db;
    //toggleFavorite toggleFav;
    public FirebaseUser user;
    private CollectionReference fs;
    private QuerySnapshot querySnapshot;
    private bool isFav;

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
        db.Collection("Restaurant").GetSnapshotAsync().ContinueWithOnMainThread(async task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data: " + task.Exception);
                return;
            }
            RestaurantsData.Clear();
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
                Debug.Log("BEFORE TOGGLE FAV DOCUMENT ID IS " + document.Id);
                await getQuerySnapshot(document.Id);
                data.Add("userFavorite", isFav);
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
        for (int i = 0; i < RestaurantsData.Count; i++)
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

    //public bool isFavoriteAsync(string ID)
    //{
    //    Debug.Log("in isfavorite");
    //    getQuerySnapshot(ID);
    //    return querySnapshot.Count != 0;
    //}

    public async Task getQuerySnapshot(string ID)
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        fs = FirebaseFirestore.DefaultInstance.Collection("Account").Document(user.UserId).Collection("Favorites");
        Query query = fs.WhereEqualTo("ID", ID);
        querySnapshot = await query.GetSnapshotAsync();
        isFav = querySnapshot.Count != 0;
    }
}
