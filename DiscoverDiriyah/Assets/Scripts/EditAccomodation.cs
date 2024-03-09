using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEditor;
using System;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Storage;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.XR;
using Firebase.Extensions;
using System.Linq;
using Bitsplash.DatePicker;

public class EditAccommodation : MonoBehaviour
{

    public string defualtAccommodationId = "exDta4AJJHBtUteWOEsR";

    FirebaseFirestore db;
    FirebaseStorage storage;
    StorageReference storageRef;
    Dictionary<string, object> Accommodation;
    public gallerySelection gallerySelection;
    List<string> pictures;
    private string AccommodationId;
    bool isValid;
    public DatePickerSettings startDatePicker;
    public TMP_Text startDateLabel;







    void Start()
    {

        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);


        //-- this line will load restaurant id from local storage in the device. 
        //-- first of all, PlayerPrefs.SetString("restId",value); will use when user click on "Edit" Button. then navigate to this scene will work.
        /*AccommodationId = PlayerPrefs.GetString("accommodationId", defualtAccommodationId); //-- load restId if not exist. will use default data.
        if (AccommodationId == null)
        {
            Debug.LogError("accommodation id not found");
        }*/
        LoadData();

    }

    private void DisplayAccommodationData(DateTime startDate)
    {
        var selection = startDatePicker.Content.Selection;
        selection.SelectOne(startDate);
        startDateLabel.text = startDate.ToString("MM/dd/yyyy");

    }
    private void LoadData()
    {
        DocumentReference docRef = db.Collection("Event").Document("hU6XU1RNNRL5WPONeCYX");

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching document: " + task.Exception);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log("Document data for document " + snapshot.Id + ": " + snapshot.ToDictionary().ToString());

                Timestamp startDateTimestamp = snapshot.GetValue<Timestamp>("StartDate");
                DateTime startDate = startDateTimestamp.ToDateTime();
                DisplayAccommodationData(startDate);

                // Now you can use startDate variable as needed
                Debug.Log("Start Date: " + startDate.ToString());

                // If you need to use startDate outside this method, you should store it in a field or property of this class or pass it to another method.
            }
            else
            {
                Debug.Log("Document does not exist!");
            }
        });
    }
}

    

    







