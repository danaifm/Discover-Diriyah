using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class AddAttraction : MonoBehaviour
{
    public bool isEdit = false;
    public string defaultId = "";
    public string titleForAdd = "Add Attraction";
    public string titleForEdit = "Edit Attraction";
    public string storageFolderName = "attractions";
    public string firestoreCollectionName = "Attraction";
    //public TMP_Text titleText;
    public TMP_InputField name;
    public TMP_Text nameError;
    public TMP_Text nameCounter;
    public TMP_InputField description;
    public TMP_Text descriptionCounter;
    public TMP_Text descriptionError;
    public TMP_InputField location;
    public TMP_Text locationError;
    public TMP_Text locationCounter;
    public TMP_Text picturesError;
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
    public TMP_Dropdown StartHour;
    public TMP_Dropdown StartMinute;
    public TMP_Dropdown StartAmPm;
    public TMP_Text startTimeError;
    public TMP_Dropdown EndHour;
    public TMP_Dropdown EndMinute;
    public TMP_Dropdown EndAmPm;
    public TMP_Text endTimeError;

    string startTime;
    string endTime;
    string workingHours; //fb
    string attractionId;

    private AttractionsRoot attractions_root = new AttractionsRoot();
    //object to upload in nuhas array

    // Start is called before the first frame update
    void Start()
    {
        alertDialog = FindObjectOfType<AlertDialog>();
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);

        pictures = gallerySelection.GetSelectedImagePaths();

        name.characterLimit = 25;
        location.characterLimit = 35;
        description.characterLimit = 250;

        //if (isEdit)
        //{
        //    titleText.text = titleForEdit;
        //    attractionId = PlayerPrefs.GetString("restId", defaultId); //-- load restId if not exist. will use default data.
        //    if (attractionId == null)
        //    {
        //        Debug.LogError("not found attraction id");
        //    }
        //    else
        //    {
        //        LoadData();
        //    }
        //}
        //else
        //    titleText.text = titleForAdd;
    }

    // Update is called once per frame
    void Update()
    {
        nameCounter.text = name.text.Length + "/" + name.characterLimit;
        locationCounter.text = location.text.Length + "/" + location.characterLimit;
        descriptionCounter.text = description.text.Length + "/" + description.characterLimit;
    }

    public void ValidateInput(TMP_InputField inputField, TMP_Text errorText, string pattern = null)
    {
        int x = 0;
        if (string.IsNullOrEmpty(inputField.text) || inputField.text.Trim() == "")
        {
            //Debug.LogError(inputField.name + " is empty");
            errorText.text = "This field cannot be empty.";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            inputField.image.color = Color.red;
            x = 1;
            isValid = false;
        }
        else if (pattern != null && !Regex.IsMatch(inputField.text, pattern))
        {
            //Debug.LogError(inputField.name + " only string allwed");
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
            //Debug.LogError(inputField.name + " is empty");
            errorText.text = "This field cannot be empty.";
            errorText.color = Color.red;
            errorText.fontSize = 3;
            inputField.image.color = Color.red;
            x = 1;
            isValid = false;
        }
        else if (pattern != null && !Regex.IsMatch(inputField.text, pattern))
        {
            //Debug.LogError(inputField.name + " Invalid location URL");
            errorText.text = "Invalid location URL";
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

    public void ValidateTime()
    {
        //START TIME VALIDATION
        if (StartHour.value == 0 && StartMinute.value == 0)
        {
            startTimeError.text = "Start hour and minute must be selected";
            startTimeError.color = Color.red;
            //startTimeError.fontSize = 30;
            isValid = false;

        }


        else if (StartHour.value == 0)
        {
            startTimeError.text = "Start hour must be selected";
            startTimeError.color = Color.red;
            //startTimeError.fontSize = 30;
            isValid = false;

        }


        else if (StartMinute.value == 0)
        {
            startTimeError.text = "Start minute must be selected";
            startTimeError.color = Color.red;
            //startTimeError.fontSize = 30;
            isValid = false;

        }
        else
        {
            startTimeError.text = "";
            string hourPart = StartHour.options[StartHour.value].text;
            string minutePart = StartMinute.options[StartMinute.value].text;
            string AmPmPart = StartAmPm.options[StartAmPm.value].text;
            startTime = hourPart + ":" + minutePart + " " + AmPmPart;
            workingHours = startTime + "-";

        }

        //END TIME VALIDATION
        if (EndHour.value == 0 && EndMinute.value == 0)
        {
            endTimeError.text = "End hour and minute must be selected";
            endTimeError.color = Color.red;
            //endTimeError.fontSize = 30;
            isValid = false;

        }

        else if (EndHour.value == 0)
        {
            endTimeError.text = "End hour must be selected";
            endTimeError.color = Color.red;
            //endTimeError.fontSize = 30;
            isValid = false;

        }


        else if (EndMinute.value == 0)
        {
            endTimeError.text = "End minute must be selected";
            endTimeError.color = Color.red;
            //endTimeError.fontSize = 30;
            isValid = false;

        }
        else
        {
            endTimeError.text = "";
            string hourPart2 = EndHour.options[EndHour.value].text;
            string minutePart2 = EndMinute.options[EndMinute.value].text;
            string AmPmPart2 = EndAmPm.options[EndAmPm.value].text;
            endTime = hourPart2 + ":" + minutePart2 + " " + AmPmPart2;
            workingHours += endTime;

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
                StorageReference fileRef = storageRef.Child(storageFolderName).Child(fileName);

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
    public void RemoveImage(int index)
    {
        //pictures.RemoveAt(index);
        gallerySelection.RemoveImage(index, storageFolderName, !isEdit);
    }
    public void SubmitButtonClick()
    {
        Validation();
        uploadEvent();
    }
    public void Validation()
    {
        isValid = true;
        ValidateInput(name, nameError);
        ValidateInput(description, descriptionError);
        string urlPattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
        ValidateLocation(location, locationError, urlPattern);
        pictures = gallerySelection.GetSelectedImagePaths();
        ValidateTime();
    }
    public async Task uploadEvent()
    {
        //-- validate pictures.
        if (pictures.Count <= 0)
        {
            isValid = false;
            Debug.Log("Images is empty");
            picturesError.text = "A picture must be uploaded.";
            picturesError.color = Color.red;
            picturesError.fontSize = 3;
        }
        else
        {
            picturesError.text = "";
        }
        if (!isValid)
        {
            return;
        }
        alertDialog.ShowLoading();
        // Assuming you have a List<string> imagePaths filled with your image paths
        List<string> uploadedImageNames = await UploadImages(pictures, name.text); // Call your UploadImages method

        var newDocument = new Dictionary<string, object>
        {
            {"Name", name.text},
            {"Location", location.text},
            {"WorkingHours", workingHours},
            {"Description", description.text},
            {"Picture", uploadedImageNames ?? new List<string>()},
        };
        try
        {
            // Assuming 'db' is already initialized Firestore instance and ready to use
            DocumentReference docRef;
            if (!isEdit)
            {
                docRef = await db.Collection("Attraction").AddAsync(newDocument);
                string newAttractionId = docRef.Id;

                //upload to nuhas array 
                if (attractions_root == null) Debug.LogError("attractions_root is null!");
                attractions_root.Name = name.text;
                attractions_root.Description = description.text;
                attractions_root.Location = location.text;
                attractions_root.Picture = uploadedImageNames;
                attractions_root.ID = newAttractionId;
                attractions_root.userFavorite = false;

                //PlayerPrefs.SetString("SelectedAttractionId", newAttractionId);
                //SceneManager.LoadScene("ViewDiscription");


            }
            else
            {
                docRef = db.Collection("Attraction").Document(attractionId);
                await docRef.UpdateAsync(newDocument);
            }
            Debug.Log($"Restaurant added successfully with ID: {docRef.Id}");
#if UNITY_EDITOR
            PlayerPrefs.SetString("restId", docRef.Id); //-- for testing purpose should remove it.
#endif

            if (uploadedImageNames != null && uploadedImageNames.Count > 0)
            {
                Debug.Log($"Uploaded {uploadedImageNames.Count} images successfully.");
                //onCompleteAddEvent.Invoke();
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
        addToUI(attractions_root); //STEP 1 CALLED HERE
    }

    //STEP 1

    private void addToUI(AttractionsRoot root)
    {
        AttractionsManager attractionsManager = gameObject.AddComponent<AttractionsManager>();
        if (attractionsManager == null) Debug.LogError("AttractionsManager is null!");
        attractionsManager.InitializeAndShowSpecificAttraction(root); //STEP 2

    }
}
