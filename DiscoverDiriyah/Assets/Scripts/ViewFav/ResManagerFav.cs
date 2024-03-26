using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Linq;
using System;

public class ResManagerFav : MonoBehaviour
{
    public static ResManagerFav Instance;
    public FirebaseAuth auth;
    public FirebaseUser user;

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

    public void OnButtonClick()
    {
        StartOnButtonClickAsync();
    }

    private async void StartOnButtonClickAsync()
    {
        try
        {
            await GetRestaurantsDataAsync();

            // Rest of your code
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred: " + ex.Message);
        }
    }
    public async Task  GetRestaurantsDataAsync()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        string userID = user.UserId;
        RestaurantsPanel.SetActive(true);
        Debug.Log("fffff");
        toggleFav = gameObject.AddComponent<toggleFavorite>();
        // Retrieve the favorites collection for the current user
        db = FirebaseFirestore.DefaultInstance;
        CollectionReference favoritesCollectionRef = db.Collection("Account").Document(userID).Collection("Favorites");

        // Query the favorites collection to filter documents where the "type" attribute is equal to "restaurant"
        QuerySnapshot favoritesSnapshot = await favoritesCollectionRef.WhereEqualTo("Type", "Restaurant").GetSnapshotAsync();
        Debug.Log("here 0");

       // RestaurantsData.Clear();
      
            foreach (var favoriteDocument in favoritesSnapshot.Documents)
        {
            string restaurantId = favoriteDocument.Id;
           
            // Retrieve the document from the "Restaurant" collection using the ID
            DocumentSnapshot restaurantDocument = await db.Collection("Restaurant").Document(restaurantId).GetSnapshotAsync();
            Debug.Log("here 1");


            if (restaurantDocument.Exists)
            {
                Dictionary<string, object> restaurantData = restaurantDocument.ToDictionary();
                Debug.Log("Restaurant document does not exist");

                if (restaurantData.ContainsKey("Pictures"))
                {
                    List<object> yourArray = (List<object>)restaurantData["Pictures"];
                    Debug.Log("here 3");
                 
                }
                isFav = await toggleFav.isFavorite(favoriteDocument.Id);
                restaurantData.Add("ID", favoriteDocument.Id);
                restaurantData.Add("userFavorite", isFav);
                string json = JsonConvert.SerializeObject(restaurantData);
                RestaurantsRoot EventsRoot = JsonUtility.FromJson<RestaurantsRoot>(json);
                RestaurantsData.Add(EventsRoot);
                // Log the JSON string
                Debug.Log("JSON data: " + json);
                //documentsList.Add(data);
                Debug.Log("here");
            }
             else
             {
            Debug.Log("Restaurant document does not exist");
              }
        }
        DataHandler();
       

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
}
