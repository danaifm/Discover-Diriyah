using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class RestaurantsManager : MonoBehaviour
{
    public static RestaurantsManager Instance;

    public List<RestaurantsRoot> RestaurantsData = new List<RestaurantsRoot>();
    public RectTransform ParentTransform;
    public GameObject RestaurantsPanel;
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
    public void GetRestaurantsData()
    {
        RestaurantsPanel.SetActive(true);
        Debug.Log("fffff");
        toggleFav = gameObject.AddComponent<toggleFavorite>();
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
                //foreach (var pair in data)
                //{
                //    Debug.Log(pair.Key + ": " + pair.Value);
                //}
                if (data.ContainsKey("Pictures"))
                {
                    List<object> yourArray = (List<object>)data["Pictures"];
                    //foreach (var item in yourArray)
                    //{
                    //    Debug.Log("Image url : " + item.ToString());
                    //}
                }
                isFav = await toggleFav.isFavorite(document.Id);
                data.Add("ID", document.Id);
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

    public void ShowSpecificRestaurant(RestaurantsRoot root)
    {
        GameObject temp = Instantiate(UI_Prefab, ParentTransform);
        temp.GetComponent<RestaurantsItem>().Init(root);

    }
    public void DiscardData()
    {
        foreach (Transform child in ParentTransform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
    }

    public void RefreshRestaurantsList() //dana
    {
        //DiscardData();
        GameObject temp;
        for (int i = 0; i < RestaurantsData.Count; i++)
        {
            temp = Instantiate(UI_Prefab, ParentTransform);
            temp.GetComponent<RestaurantsItem>().Init(RestaurantsData[i]);
        }

    }

    public void InitializeAndShowSpecificRestaurant(RestaurantsRoot restaurantsRoot) //STEP 2
    {
        RestaurantsData.Add(restaurantsRoot); //important to add to the list for search (it searches from the list and uses Count())
        SceneManager.UnloadSceneAsync("AddRestaurant"); //unload the add attraction scene
        ShowSpecificRestaurant(restaurantsRoot); //shows the attraction in the attractionx list ui
        RestaurantDescriptionImagesManager.Instance.ShowDescription(restaurantsRoot); //STEP 3: show the attraction details
        Debug.Log("end of initialize and show method");
    }
}
