using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class EditAccommodation : MonoBehaviour
{

    public TMP_InputField Name;
    public TMP_Text nameError;
    public TMP_Text nameCounter;
    public TMP_Text descriptionCounter;
    public TMP_Text locationCounter;
    public TMP_Text starRatingCounter;
    public TMP_InputField Description;
    public TMP_Text descriptionError;
    public TMP_InputField StarRating;
    public TMP_Text starRatingError;
    public TMP_InputField Location;
    public TMP_Text locationError;
    public TMP_Text pictureError;
    public string defaultAccommodationID;
    string name; //fb
    string description;//fb 
    string rating;//fb
    double starRating; 
    string location;//fb
    

    FirebaseFirestore db;
    FirebaseStorage storage;
    StorageReference storageRef;
    Dictionary<string, object> Accommodation;
    public gallerySelection gallerySelection;
    List<string> pictures;


    bool isValid = true;

    private string accommodationId;
    public AlertDialog alertDialog;
    public UnityEvent onCompleteAddEvent;


    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);

        pictures = gallerySelection.GetSelectedImagePaths();

        Name.characterLimit = 25;
        Description.characterLimit = 250;
        Location.characterLimit = 35;
        StarRating.characterLimit = 4;

        accommodationId = PlayerPrefs.GetString("accommodationID", defaultAccommodationID); //-- load restId if not exist. will use default data.
        if (accommodationId == null || accommodationId == "")
        {
            Debug.LogError("not found accommodation id");
        }
        
        LoadData();
    }

private void LoadData()
    {
        DocumentReference docRef = db.Collection("Accommodation").Document(accommodationId);
        alertDialog.ShowLoading();
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log($"Document data for {snapshot.Id} document:");

                Accommodation accommodation = new Accommodation();

                accommodation.Name = snapshot.GetValue<string>("Name");
                Debug.Log("1");
                accommodation.Location = snapshot.GetValue<string>("Location");
                Debug.Log("2");
                accommodation.Description = snapshot.GetValue<string>("Description");
                Debug.Log("3");
                accommodation.StarRating = snapshot.GetValue<double>("StarRating");
                Debug.Log("4");
                string[] pictures = snapshot.GetValue<string[]>("Picture");
                accommodation.Pictures = pictures.ToList<string>();
                Debug.Log("5");
                DisplayAccommodationData(accommodation);
                alertDialog.HideLoading();
            }
            else
            {
                Debug.Log($"Document {snapshot.Id} does not exist!");
                alertDialog.HideLoading();
            }
        });
    }

    private void DisplayAccommodationData(Accommodation accommodation)
    {
        Name.text = accommodation.Name;
        Location.text = accommodation.Location;
        gallerySelection.DisplayLoadedImages(accommodation.Pictures, "accommodations");
        Description.text = accommodation.Description;
        StarRating.text = accommodation.StarRating.ToString();
        // Handle pictures if needed
    }

    void Update()
    {
        nameCounter.text = Name.text.Length + "/" + Name.characterLimit;
        descriptionCounter.text = Description.text.Length + "/" + Description.characterLimit;
        locationCounter.text = Location.text.Length + "/" + Location.characterLimit;
        starRatingCounter.text = StarRating.text.Length + "/" + StarRating.characterLimit;
    }

    public void validate_input()
    {
        bool isValid = true;

        //NAME FIELD VALIDATION
        name = Name.text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            nameError.text = "This field cannot be empty";
            nameError.color = Color.red;
            nameError.fontSize = 3;
            isValid = false;

        }
        else
        {
            nameError.text = "";
        }

        //DESCRIPTION FIELD VALIDATION
        description = Description.text.Trim();
        if (string.IsNullOrWhiteSpace(description))
        {
            descriptionError.text = "This field cannot be empty";
            descriptionError.color = Color.red;
            descriptionError.fontSize = 3;
            isValid = false;

        }
        else
        {
            descriptionError.text = "";
        }

        //STAR RATING FIELD VALIDATION
        rating = StarRating.text.Trim();
        string ratingPattern = @"^(5(\.0)?|[0-4](\.\d)?)$";

        if (!Regex.IsMatch(rating, ratingPattern))
        {
            starRatingError.text = "Invalid Star Rating";
            starRatingError.color = Color.red;
            starRatingError.fontSize = 3;
            isValid = false;

        }

        else if (string.IsNullOrWhiteSpace(rating))
        {
            starRatingError.text = "This field cannot be empty";
            starRatingError.color = Color.red;
            starRatingError.fontSize = 3;
            isValid = false;

        }
        else
        {
            starRatingError.text = "";
            starRating = double.Parse(rating);
        }

        //LOCATION FIELD VALIDATION
        location = Location.text.Trim();
        string urlPattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";

        if (!Regex.IsMatch(location, urlPattern))
        {
            locationError.text = "Invalid location URL";
            locationError.color = Color.red;
            locationError.fontSize = 3;
            isValid = false;

        }
        else if (string.IsNullOrWhiteSpace(location))
        {
            locationError.text = "This field cannot be empty";
            locationError.color = Color.red;
            locationError.fontSize = 3;
            isValid = false;

        }
        else
        {
            locationError.text = "";
        }
        
        pictures = gallerySelection.GetSelectedImagePaths();

        //PICTURE VALIDATION
        /*if(pictures.Count == 0)
        {
            pictureError.text = "This field cannot be empty";
            pictureError.color = Color.red;
            pictureError.fontSize = 3;
            isValid = false;

        }*/

    }//end of validations 

    public void RemoveImage(int index)
    {
        //pictures.RemoveAt(index);
        gallerySelection.RemoveImage(index, "accommodations", false);
    }


    public async Task<List<string>> UploadImages(List<string> imagePaths, string name)
    {
        if (imagePaths == null) return null;

        List<string> uploadedImageNames = new List<string>();
        int imageCounter = 1; // Start naming images from 1

        foreach (string path in imagePaths)
        {
            if (File.Exists(path))
            {
                byte[] imageBytes = File.ReadAllBytes(path);
                string fileExtension = Path.GetExtension(path).ToLower(); // Get the file extension in lowercase
                string contentType = "image/jpeg"; // Default content type

                // Set the content type based on the file extension
                if (fileExtension == ".png")
                {
                    contentType = "image/png";
                }
                else if (fileExtension == ".jpg" || fileExtension == ".jpeg")
                {
                    contentType = "image/jpeg";
                }

                string fileName = $"{name}{imageCounter}{fileExtension}";
                StorageReference fileRef = storageRef.Child("accommodations").Child(fileName);

                // Upload the image bytes to Firebase Storage with the determined content type
                var metadata = new MetadataChange() { ContentType = contentType };
                await fileRef.PutBytesAsync(imageBytes, metadata);

                // Optionally, get the download URL
                Uri downloadUri = await fileRef.GetDownloadUrlAsync();
                string downloadUrl = downloadUri.ToString();


                if (!string.IsNullOrEmpty(downloadUrl))
                {
                    uploadedImageNames.Add(fileName);
                    imageCounter++; // Increment for the next image
                }
            }
            else
            {
                uploadedImageNames.Add(path);
            }
        }

        return uploadedImageNames;
    }


    public void SubmitButtonClick()
    {
        validate_input();
        uploadEvent();
    }
    public async Task uploadEvent()
    {
        if (pictures.Count <= 0)
        {
            isValid = false;
            Debug.LogError("Images is empty");
            pictureError.text = "A picture must be uploaded.";
            pictureError.color = Color.red;
            pictureError.fontSize = 3;
        }
        else
        {
            pictureError.text = "";
        }
        if (!isValid) return;
        alertDialog.ShowLoading();
        // Assuming you have a List<string> imagePaths filled with your image paths
        List<string> uploadedImageNames = await UploadImages(pictures, Name.text); // Call your UploadImages method

        var newAccommodation = new Dictionary<string, object>
        {
            {"Name", Name.text},
            {"Location", Location.text},
            {"Description", Description.text},
            {"StarRating", StarRating.text},
            // Add an empty array if uploadedImageNames is null or empty
            {"Picture", uploadedImageNames.ToArray()}
        };
        try
        {
            // Assuming 'db' is already initialized Firestore instance and ready to use

            var docRef = db.Collection("Accommodation").Document(accommodationId);

            await docRef.UpdateAsync(newAccommodation);
            Debug.Log($"Accommodation updated successfully with ID: {docRef.Id}");
            onCompleteAddEvent.Invoke();
            alertDialog.HideLoading();


        }
        catch (Exception ex)
        {
            Debug.LogError($"Error adding Accommodation: {ex.Message}");
            alertDialog.HideLoading();
        }
    }
}