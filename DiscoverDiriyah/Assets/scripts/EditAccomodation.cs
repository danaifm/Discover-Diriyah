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

public class EditAccommodation : MonoBehaviour
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

    public string defualtAccommodationId = "JVtPSgcY4Tzy2DNcVDUe";

    FirebaseFirestore db;
    FirebaseStorage storage;
    StorageReference storageRef;
    Dictionary<string, object> Accommodation;
    public gallerySelection gallerySelection;
    List<string> pictures;
    private string AccommodationId;





    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);

        pictures = gallerySelection.GetSelectedImagePaths();

        //-- this line will load restaurant id from local storage in the device. 
        //-- first of all, PlayerPrefs.SetString("restId",value); will use when user click on "Edit" Button. then navigate to this scene will work.
        AccommodationId = PlayerPrefs.GetString("accommodationId", defualtAccommodationId); //-- load restId if not exist. will use default data.
        if (AccommodationId == null)
        {
            Debug.LogError("accommodation id not found");
        }
        LoadData();

    }

    private void LoadData()
    {
        DocumentReference docRef = db.Collection("Accommodation").Document(AccommodationId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log($"Document data for {snapshot.Id} document:");
                Accommodation accommodation = new Accommodation();

                // Map data from the snapshot to the Restaurant object
                accommodation.Name = snapshot.GetValue<string>("Name");
                accommodation.Location = snapshot.GetValue<string>("Location");
                accommodation.Description = snapshot.GetValue<string>("Description");
                accommodation.StarRating = snapshot.GetValue<string>("StarRating");
                string[] pictures = snapshot.GetValue<string[]>("Picture");
                accommodation.Pictures = pictures.ToList<string>();

                DisplayAccommodationData(accommodation);
            }
            else
            {
                Debug.Log($"Document {snapshot.Id} does not exist!");
            }
        });
    }

    private void DisplayAccommodationData(Accommodation accommodation)
    {
        Name.text = accommodation.Name;
        Location.text = accommodation.Location;
        Description.text = accommodation.Description;
        StarRating.text = accommodation.StarRating;
        gallerySelection.DisplayLoadedImages(accommodation.Pictures);
    }

    public void RemoveImage(int index)
    {
        //pictures.RemoveAt(index);
        gallerySelection.RemoveImage(index);
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
        if (pictures.Count == 0)
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

                else
                {
                    uploadedImageNames.Add(path);
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
        {"Name", Name.text},
        {"Description", Description.text},
        {"StarRating", StarRating.text},
        {"Location", Location.text},
        // Add an empty array if uploadedImageNames is null or empty
        {"Picture", uploadedImageNames.ToArray()}
    };

        try
        {
            // Assuming 'db' is already initialized Firestore instance and ready to use

            var docRef = db.Collection("Accommodation").Document(AccommodationId);

            await docRef.UpdateAsync(newAccommodation);
            Debug.Log($"Accommodation updated successfully with ID: {docRef.Id}");

        }
        catch (Exception ex)
        {
            Debug.LogError($"Error Editing Accommodation: {ex.Message}");
        }
    }




}


