using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;

public class AttractionsManager : MonoBehaviour
{
    public static AttractionsManager Instance;

    public List<AttractionsRoot> AttractionsData = new List<AttractionsRoot>();
    public RectTransform ParentTransform;
    public GameObject AttractionsPanel;
    public GameObject UI_Prefab;

    [Header("Firebase Storage url")]
    public string FirebaseStorageUrl;

    FirebaseFirestore db;

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
    public void GetAttractionsData()
    {
        AttractionsPanel.SetActive(true);
        Debug.Log("fffff");
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
                await isFavoriteAsync(document.Id);
                data.Add("userFavorite", isFav);
                string json = JsonConvert.SerializeObject(data);
                AttractionsRoot AttractionsRoot = JsonUtility.FromJson<AttractionsRoot>(json);
                AttractionsData.Add(AttractionsRoot);
                // Log the JSON string
                Debug.Log("JSON data: " + json);
                //documentsList.Add(data);
                Debug.Log("/////////////////////////////////");
            }
            DataHandler();
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
    public void DiscardData()
    {
        foreach (Transform child in ParentTransform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
    }

    public async Task isFavoriteAsync(string ID)
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        fs = FirebaseFirestore.DefaultInstance.Collection("Account").Document(user.UserId).Collection("Favorites");
        Query query = fs.WhereEqualTo("ID", ID);
        querySnapshot = await query.GetSnapshotAsync();
        isFav = querySnapshot.Count != 0;
    }

}
