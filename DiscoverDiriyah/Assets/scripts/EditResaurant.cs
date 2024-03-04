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

public class EditResaurant : MonoBehaviour
{

    public TMP_InputField name;
    public TMP_Text nameError;
    public TMP_Text nameCounter;
    public TMP_InputField cuisineType;
    public TMP_Text cuisineError;
    public TMP_Text cuisineCounter;
    public TMP_InputField location;
    public TMP_Text locationError;
    public TMP_Text locationCounter;
    public TMP_Text picturesError;
    public string defualtRestId = "atpMSmgVdhkVJ1WBKWTt";
    FirebaseFirestore db;
    FirebaseStorage storage;
    StorageReference storageRef;
    Dictionary<string, object> Event;
    public gallerySelection gallerySelection;

    List<string> pictures;
    bool isValid = true;

    private string restId;
    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);

        pictures = gallerySelection.GetSelectedImagePaths();

        name.characterLimit = 25;
        cuisineType.characterLimit = 25;
        location.characterLimit = 35;

        //-- this line will load restaurant id from local storage in the device. 
        //-- first of all, PlayerPrefs.SetString("restId",value); will use when user click on "Edit" Button. then navigate to this scene will work.
        restId = PlayerPrefs.GetString("restId", defualtRestId); //-- load restId if not exist. will use default data.
        if (restId == null)
        {
            Debug.LogError("not found restaurant id");
        }
        LoadData();
    }

    private void LoadData()
    {
        DocumentReference docRef = db.Collection("Restaurant").Document(restId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log($"Document data for {snapshot.Id} document:");
                Restaurant restaurant = new Restaurant();

                // Map data from the snapshot to the Restaurant object
                restaurant.Name = snapshot.GetValue<string>("Name");
                restaurant.Location = snapshot.GetValue<string>("Location");
                restaurant.CuisineType = snapshot.GetValue<string>("CuisineType");
                string[] pictures = snapshot.GetValue<string[]>("Picture");
                restaurant.Pictures = pictures.ToList<string>();

                DisplayRestaurantData(restaurant);
            }
            else
            {
                Debug.Log($"Document {snapshot.Id} does not exist!");
            }
        });
    }

    private void DisplayRestaurantData(Restaurant restaurant)
    {
        name.text = restaurant.Name;
        location.text = restaurant.Location;
        cuisineType.text = restaurant.CuisineType;
        gallerySelection.DisplayLoadedImages(restaurant.Pictures);
        // Handle pictures if needed
    }

    private void Update()
    {
        nameCounter.text = name.text.Length + "/" + name.characterLimit;
        cuisineCounter.text = cuisineType.text.Length + "/" + cuisineType.characterLimit;
        locationCounter.text = location.text.Length + "/" + location.characterLimit;
    }
    public void Validation()
    {
        isValid = true;
        ValidateInput(name, nameError);
        string urlPattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
        ValidateLocation(location, locationError, urlPattern);

        string pattern3 = @"^[a-zA-Z ]*$";
        ValidateInput(cuisineType, cuisineError, pattern3);
        pictures = gallerySelection.GetSelectedImagePaths();
    }
    public void RemoveImage(int index)
    {
        //pictures.RemoveAt(index);
        gallerySelection.RemoveImage(index);
    }
    public void ValidateInput(TMP_InputField inputField, TMP_Text errorText, string pattern = null)
    {
        int x = 0;
        if (string.IsNullOrEmpty(inputField.text) || inputField.text.Trim() == "")
        {
            Debug.LogError(inputField.name + " is empty");
            errorText.text = "This field cannot be empty.";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            inputField.image.color = Color.red;
            x = 1;
            isValid = false;
        }
        else if (pattern != null && !Regex.IsMatch(inputField.text, pattern))
        {
            Debug.LogError(inputField.name + " only string allwed");
            errorText.text = "Only string allowed";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            inputField.image.color = Color.red;
            x = 1;
            isValid = false;
        }
        else
        {
            errorText.text = "";
            inputField.image.color = Color.gray;
        }
    }

public void ValidateLocation(TMP_InputField inputField, TMP_Text errorText, string pattern = null)
    {
        int x = 0;
        if (string.IsNullOrEmpty(inputField.text) || inputField.text.Trim() == "")
        {
            Debug.LogError(inputField.name + " is empty");
            errorText.text = "This field cannot be empty.";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            inputField.image.color = Color.red;
            x = 1;
            isValid = false;
        }else if (pattern != null && !Regex.IsMatch(inputField.text, pattern))
        {
            Debug.LogError(inputField.name + " Invalid location URL");
            errorText.text = "Invalid location URL";
            errorText.fontSize = 3;
            errorText.color = Color.red;
            inputField.image.color = Color.red;
            x = 1;
            isValid = false;
        }
        else
        {
            errorText.text = "";
            inputField.image.color = Color.gray;
        }
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
                StorageReference fileRef = storageRef.Child("restaurant").Child(fileName);

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
        Validation();
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
        // Assuming you have a List<string> imagePaths filled with your image paths
        List<string> uploadedImageNames = await UploadImages(pictures, name.text); // Call your UploadImages method

        var newRestaurant = new Dictionary<string, object>
        {
            {"Name", name.text},
            {"Location", location.text},
            {"CuisineType", cuisineType.text},
            // Add an empty array if uploadedImageNames is null or empty
            {"Picture", uploadedImageNames.ToArray()}
        };
        try
        {
            // Assuming 'db' is already initialized Firestore instance and ready to use

            var docRef = db.Collection("Restaurant").Document(restId);

            await docRef.UpdateAsync(newRestaurant);
            Debug.Log($"Restaurant updated successfully with ID: {docRef.Id}");
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error adding Restaurant: {ex.Message}");
        }
    }
}
