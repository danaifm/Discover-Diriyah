using Firebase.Firestore;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class AddAccommodationDetails : MonoBehaviour
{
    public TMP_InputField Name;
    public TMP_Text nameError;
    public TMP_Text nameCounter;
    public TMP_InputField Description;
    public TMP_Text descriptionError;
    public TMP_Text descriptionCounter;
    public TMP_InputField StarRating;
    public TMP_Text starRatingError;
    public TMP_Text starRatingCounter;
    public TMP_InputField Location;
    public TMP_Text locationError;
    public TMP_Text locationCounter;
    public TMP_Text picturesError;
    string name; //fb
    string description;//fb 
    string rating;//fb
    double starRating; 
    string location;//fb
    // UnityEvent to be invoked on button click
    FirebaseFirestore db;
    FirebaseStorage storage;
    StorageReference storageRef;
    Dictionary<string, object> Event;
    public gallerySelection gallerySelection;
    
    List<string> pictures;
    bool isValid = true;
    public AlertDialog alertDialog;
    public UnityEvent onCompleteAddEvent;
    // Start is called before the first frame update
    void Start()
    {
        alertDialog = FindObjectOfType<AlertDialog>();
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);

        pictures = gallerySelection.GetSelectedImagePaths();

        Name.characterLimit = 25;
        Description.characterLimit = 250;
        Location.characterLimit = 35;
        StarRating.characterLimit = 4;
    }

    // Update is called once per frame
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
        gallerySelection.RemoveImage(index, "accommodations");
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
            picturesError.text = "A picture must be uploaded.";
            picturesError.color = Color.red;
            picturesError.fontSize = 3;
        }
        else
        {
            picturesError.text = "";
        }
        if (!isValid) return;
        alertDialog.ShowLoading();
        // Assuming you have a List<string> imagePaths filled with your image paths
        List<string> uploadedImageNames = await UploadImages(pictures, name);

        var newAccommodation = new Dictionary<string, object>
    {
        {"Name", name},
        {"Description", description},
        {"StarRating", starRating},
        {"Location", location},
        // Add an empty array if uploadedImageNames is null or empty
        {"Picture", uploadedImageNames ?? new List<string>()}
    };
        try
        {
            // Assuming 'db' is already initialized Firestore instance and ready to use
            var docRef = await db.Collection("Accommodation").AddAsync(newAccommodation);
            Debug.Log($"Accommodation added successfully with ID: {docRef.Id}");

            if (uploadedImageNames != null && uploadedImageNames.Count > 0)
            {
                Debug.Log($"Uploaded {uploadedImageNames.Count} images successfully.");
                onCompleteAddEvent.Invoke();
                alertDialog.HideLoading();
            }
            else
            {
                Debug.Log("No images were uploaded.");
                alertDialog.HideLoading();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error adding Restaurant: {ex.Message}");
            alertDialog.HideLoading();
        }
    }
}
