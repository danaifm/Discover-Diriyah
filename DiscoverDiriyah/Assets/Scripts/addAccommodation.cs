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

public class addAccommodation : MonoBehaviour
{

    public TMP_InputField Name;
    public TMP_Text nameError;
    public TMP_InputField Description;
    public TMP_Text descriptionError;
    public TMP_InputField StarRating;
    public TMP_Text starRatingError;
    public TMP_InputField Location;
    public TMP_Text locationError;
    public TMP_Text pictureError;
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




    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);

        pictures = gallerySelection.GetSelectedImagePaths();

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
            nameError.fontSize = 30;
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
            descriptionError.fontSize = 30;
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
            starRatingError.fontSize = 30;
            isValid = false;

        }

        else if (string.IsNullOrWhiteSpace(rating))
        {
            starRatingError.text = "This field cannot be empty";
            starRatingError.color = Color.red;
            starRatingError.fontSize = 30;
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
            locationError.fontSize = 30;
            isValid = false;

        }
        else if (string.IsNullOrWhiteSpace(location))
        {
            locationError.text = "This field cannot be empty";
            locationError.color = Color.red;
            locationError.fontSize = 30;
            isValid = false;

        }
        else
        {
            locationError.text = "";
        }

        //PICTURE VALIDATION
        if(pictures.Count == 0)
        {
            pictureError.text = "This field cannot be empty";
            pictureError.color = Color.red;
            pictureError.fontSize = 30;
            isValid = false;

        }


        //if everything is valid -> upload to firebase 
        if (isValid)
        {
            uploadAccommodation();
        }

    }//end of validations 


    //upload pictures in storage
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



    public async Task uploadAccommodation()
    {
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
            var docRef = await db.Collection("Accommodation").AddAsync(newAccommodation);
            Debug.Log($"Accommodation added successfully with ID: {docRef.Id}");

            if (uploadedImageNames != null && uploadedImageNames.Count > 0)
            {
                Debug.Log($"Uploaded {uploadedImageNames.Count} images successfully.");
            }
            else
            {
                Debug.Log("No images were uploaded.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error adding accommodation: {ex.Message}");
        }
    }




}


