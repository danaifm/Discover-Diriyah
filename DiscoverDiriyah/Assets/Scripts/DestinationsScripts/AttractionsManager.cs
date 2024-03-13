using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

public class AttractionsManager : MonoBehaviour
{
    public static AttractionsManager Instance;

    public static List<AttractionsRoot> AttractionsData = new List<AttractionsRoot>();
    public RectTransform ParentTransform;
    public GameObject AttractionsPanel;
    public GameObject UI_Prefab;

    [Header("Firebase Storage url")]
    public string FirebaseStorageUrl;

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
    public void GetAttractionsData()
    {
        AttractionsPanel.SetActive(true);
        if (AttractionsData.Count != 0)
        {
            DataHandler();
            return;
        }
        Debug.Log("fffff");
        toggleFav = gameObject.AddComponent<toggleFavorite>();
        db = FirebaseFirestore.DefaultInstance;
        db.Collection("Attraction").GetSnapshotAsync().ContinueWithOnMainThread(async task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data: " + task.Exception);
                return;
            }
            AttractionsData.Clear();
            foreach (var document in task.Result.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();
               
                if (data.ContainsKey("Pictures"))
                {
                    List<object> yourArray = (List<object>)data["Pictures"];
                   
                }
                isFav = await toggleFav.isFavorite(document.Id);
                data.Add("userFavorite", isFav);
                string json = JsonConvert.SerializeObject(data);
                AttractionsRoot AttractionsRoot = JsonUtility.FromJson<AttractionsRoot>(json);
                AttractionsData.Add(AttractionsRoot);
                // Log the JSON string
                Debug.Log("JSON data: " + json);
                //documentsList.Add(data);
                ShowSpecificAttraction(AttractionsRoot);
                Debug.Log("/////////////////////////////////");
            }
            //DataHandler();
        });
    }
    public void DataHandler()
    {
        AttractionsPanel.SetActive(true);
        Debug.Log("DataHandler " + AttractionsData.Count);
        GameObject temp;
        for (int i = 0; i < AttractionsData.Count; i++)
        {
            temp = Instantiate(UI_Prefab, ParentTransform);
            temp.GetComponent<AttractionsItem>().Init(AttractionsData[i]);
        }
    }

    public void ShowSpecificAttraction(AttractionsRoot root)
    {
        GameObject temp = Instantiate(UI_Prefab, ParentTransform);
        temp.GetComponent<AttractionsItem>().Init(root);

    }
    public void DiscardData()
    {
        foreach (Transform child in ParentTransform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
    }

    public void InitializeAndShowSpecificAttraction(AttractionsRoot attractionsRoot) //STEP 2
    {
        AttractionsData.Add(attractionsRoot); //important to add to the list for search (it searches from the list and uses Count())
        SceneManager.UnloadSceneAsync("AddAttraction"); //unload the add attraction scene
        ShowSpecificAttraction(attractionsRoot); //shows the attraction in the attractionx list ui
        DescriptionImagesManager.Instance.ShowDescription(attractionsRoot); //STEP 3: show the attraction details
        Debug.Log("end of initialize and show method");
    }


}
