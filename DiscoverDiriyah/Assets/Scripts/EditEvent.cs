using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using Bitsplash.DatePicker;
using UI.Dates;
using UnityEditor;
using System;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Storage;
using System.Threading.Tasks;
using System.IO;
using Firebase.Extensions;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EditEvent : MonoBehaviour
{
    public bool isEdit = false;
    public string defualtId = null;
    public string firestoreCollectionName;
    public string storageFolderName;
    public AlertDialog alertDialog;
    public string titleForAdd = "Add Event";
    public string titleForEdit = "Edit Event";
    public TMP_InputField Name;
    public TMP_Text nameError;
    public TMP_InputField Description;
    public TMP_Text descriptionError;
    public TMP_InputField Audience;
    public TMP_Text audienceError;
    public TMP_Dropdown StartHour;
    public TMP_Dropdown StartMinute;
    public TMP_Dropdown StartAmPm;
    public TMP_Text startTimeError;
    public TMP_Dropdown EndHour;
    public TMP_Dropdown EndMinute;
    public TMP_Dropdown EndAmPm;
    public TMP_Text endTimeError;
    public TMP_InputField Location;
    public TMP_Text locationError;
    public TMP_Text startDateError;
    public TMP_Text endDateError;
    public DatePickerSettings startDatePicker;
    public DatePickerSettings endDatePicker;
    public TMP_InputField Price;
    public TMP_Text priceError;
    public TMP_Text pictureError;
    string name; //fb
    string description;//fb 
    string audience;//fb
    string location;//fb
    string startTime;
    string endTime;
    string workingHours; //fb
    DateTime finalStartDate; //fb
    DateTime finalEndDate; //fb
    string price;//fb 

    public TMP_Text startDateLabel;
    public TMP_Text endDateLabel;


    public UnityEvent onCompleteAddEvent;

    FirebaseFirestore db;
    FirebaseStorage storage;
    StorageReference storageRef;
    Dictionary<string, object> Event;
    public gallerySelection gallerySelection; 
    List<string> pictures;

    private string docId;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);


        //DISABLE DATES FROM PAST 6 MONTHS
        /*if (endDatePicker != null)
        {
            endDatePicker.Content.OnDisplayChanged.AddListener(() => OnDisplayChanged(endDatePicker));
        }
        if (startDatePicker != null)
        {
            startDatePicker.Content.OnDisplayChanged.AddListener(() => OnDisplayChanged(startDatePicker));
        }*/

        pictures = gallerySelection.GetSelectedImagePaths();
        if (isEdit)
        {
            docId = PlayerPrefs.GetString("event", defualtId);
            LoadData();
        }
        Name.characterLimit = 25;
        Description.characterLimit = 250;
        Location.characterLimit = 35;
        Price.characterLimit = 10;
        Audience.characterLimit = 25;
    }

/// <summary>

/// </summary>
/// <param name="calendar"></param>/
// showing Date Section //
    private void DisplayDateData(DateTime startDate, DateTime endDate)
    {
        var startDselection = startDatePicker.Content.Selection;
        var endDselection = startDatePicker.Content.Selection;
        startDselection.SelectOne(startDate);
        endDselection.SelectOne(endDate);
        startDateLabel.text = startDate.ToString("MM/dd/yyyy");
        endDateLabel.text = endDate.ToString("MM/dd/yyyy");
    }
////////////////////////////
    public void OnDisplayChanged(DatePickerSettings calendar)
    {
        DateTime currentDate = DateTime.Now;
        DateTime startDate = currentDate.AddMonths(-6);

        // Loop from 6 months ago to yesterday
        for (DateTime date = startDate; date < currentDate; date = date.AddDays(1))
        {
            var cell = calendar.Content.GetCellObjectByDate(date);
            if (cell != null)
            {
                cell.CellEnabled = false;
            }
        }

    }
    public void RemoveImage(int index)
    {
        //pictures.RemoveAt(index);
        
        gallerySelection.RemoveImage(index, storageFolderName, !isEdit);
    }

    public void SubmitClickButton()
    {
        validate_input();
        uploadEvent();

    }
    public void validate_input()
    {
        bool isValid = true;

        //NAME FIELD VALIDATION
        name = Name.text.Trim();
        string pattern1 = @"^[a-zA-Z0-9 \-\[\]\(\),]*$";
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
        string pattern2 = @"^[a-zA-Z0-9 \-\[\],:;?!().]*$";
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

        //AUDIENCE FIELD VALIDATION
        audience = Audience.text.Trim();
        string pattern3 = @"^[a-zA-Z0-9 \-\+\(\)]*$";
        if (string.IsNullOrWhiteSpace(audience))
        {
            audienceError.text = "This field cannot be empty";
            audienceError.color = Color.red;
            audienceError.fontSize = 3;
            isValid = false;

        }
        else
        {
            audienceError.text = "";
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

        //START TIME VALIDATION
        if(StartHour.value == 0 && StartMinute.value == 0)
        {
            startTimeError.text = "Start hour and minute cannot be empty";
            startTimeError.color = Color.red;
            startTimeError.fontSize = 3;
            isValid = false;

        }
        
       
        else if (StartHour.value == 0)
        {
            startTimeError.text = "Start hour cannot be empty";
            startTimeError.color = Color.red;
            startTimeError.fontSize = 3;
            isValid = false;

        }
        

        else if (StartMinute.value == 0)
        {
            startTimeError.text = "Start minute cannot be empty";
            startTimeError.color = Color.red;
            startTimeError.fontSize = 3;
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
            endTimeError.text = "End hour and minute cannot be empty";
            endTimeError.color = Color.red;
            endTimeError.fontSize = 3;
            isValid = false;

        }
        
        else if (EndHour.value == 0)
        {
            endTimeError.text = "End hour cannot be empty";
            endTimeError.color = Color.red;
            endTimeError.fontSize = 3;
            isValid = false;

        }
        

        else if (EndMinute.value == 0)
        {
            endTimeError.text = "End minute cannot be empty";
            endTimeError.color = Color.red;
            endTimeError.fontSize = 3;
            isValid = false;

        }
        else
        { 
            endTimeError.text = "";
            string hourPart2 = EndHour.options[EndHour.value].text;
            string minutePart2 = EndMinute.options[EndMinute.value].text;
            string AmPmPart2 = EndAmPm.options[EndAmPm.value].text;
            endTime = hourPart2+":"+minutePart2 + " " +AmPmPart2;
            workingHours += endTime;

        }


        DateTime currentDate = DateTime.Today; // Gets today's date with time component set to 00:00:00

        // START DATE VALIDATION
        DateTime? selectedStartDate = null; 
        if (startDatePicker != null && startDatePicker.Content.Selection != null && startDatePicker.Content.Selection.Count > 0)
        {
            var selection = startDatePicker.Content.Selection.GetItem(0); // returns a DateTime
            selectedStartDate = selection;
            /*if (selection >= currentDate)
            {
                startDateError.text = "";
            }
            else //date from past 
            {
                startDateError.text = "Start date cannot be in the past.";
                startDateError.color = Color.red;
                startDateError.fontSize = 30;
                isValid = false;
            }*/
        }
        else //field left empty
        {
            startDateError.text = "Start date cannot be empty";
            startDateError.color = Color.red;
            startDateError.fontSize = 3;
            isValid = false;
        }

        // END DATE VALIDATION 
        DateTime? selectedEndDate = null; 
        if (endDatePicker != null && endDatePicker.Content.Selection != null && endDatePicker.Content.Selection.Count > 0)
        {
            var selection = endDatePicker.Content.Selection.GetItem(0); //returns a DateTime
            selectedEndDate = selection;
            /*if (selection >= currentDate)
            {
                endDateError.text = "";
            }
            else //past date
            {
                endDateError.text = "End date cannot be in the past.";
                endDateError.color = Color.red;
                endDateError.fontSize = 30;
                isValid = false;
            }*/
        }
        else //field left empty
        {
            endDateError.text = "End date cannot be empty";
            endDateError.color = Color.red;
            endDateError.fontSize = 3;
            isValid = false;
        }

        // validation for end date not before start date
        if (selectedStartDate.HasValue && selectedEndDate.HasValue)
        {
            if (selectedEndDate.Value < selectedStartDate.Value)
            {
                endDateError.text = "End date cannot be before start date.";
                endDateError.color = Color.red;
                endDateError.fontSize = 3;
                isValid = false;
            }
            else
            {
                finalStartDate = selectedStartDate.Value;
                finalEndDate = selectedEndDate.Value;
                //finalStartDate = startDatePicker.Content.Selection.GetItem(0).ToString();
                //finalStartDate = finalStartDate[..9];
                //finalEndDate = endDatePicker.Content.Selection.GetItem(0).ToString();
                //finalEndDate = finalEndDate[..9];
            }
        }

        //PRICE FIELD VALIDATION
        price = Price.text.Trim();
        string  pricePattern= @"^-?\d+(\.\d+)?$";
        if (!Regex.IsMatch(price, pricePattern))
        {
            priceError.text = "Invalid price";
            priceError.color = Color.red;
            priceError.fontSize = 3;
            isValid = false;

        }
        else if (string.IsNullOrWhiteSpace(price))
        {
            priceError.text = "This field cannot be empty";
            priceError.color = Color.red;
            priceError.fontSize = 3;
            isValid = false;

        }
        else
        {
            priceError.text = "";
            price += " SAR";
        }

        //PICTURE VALIDATION
       /* if (pictures.Count == 0)
        {
            pictureError.text = "This field cannot be empty";
            pictureError.color = Color.red;
            pictureError.fontSize = 3;
            isValid = false;

        }*/


        //if everything is valid -> upload to firebase 
        if (isValid)
        {
            uploadEvent();
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
                StorageReference fileRef = storageRef.Child("events").Child(fileName);

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



    public async Task uploadEvent()
    {
        // Assuming you have a List<string> imagePaths filled with your image paths
        List<string> uploadedImageNames = await UploadImages(pictures,name); 

        var newEvent = new Dictionary<string, object>
    {
        {"Name", name},
        {"Description", description},
        {"Audience", audience},
        {"StartDate", finalStartDate},
        {"EndDate", finalEndDate},
        {"Location", location},
        {"Price", price},
        {"WorkingHours", workingHours},
        // Add an empty array if uploadedImageNames is null or empty
        {"Picture", uploadedImageNames ?? new List<string>()} 
    };
        try
        {
            alertDialog.ShowLoading();
            DocumentReference docRef;
            if (isEdit)
            {
                docRef = db.Collection(firestoreCollectionName).Document(docId);
                await docRef.UpdateAsync(newEvent);
            }
            else
            {

                docRef = await db.Collection(firestoreCollectionName).AddAsync(newEvent);
            }
            Debug.Log($"Event added successfully with ID: {docRef.Id}");

            if (uploadedImageNames != null && uploadedImageNames.Count > 0)
            {
                Debug.Log($"Uploaded {uploadedImageNames.Count} images successfully.");
            }
            else
            {
                Debug.Log("No images were uploaded.");
            }
            alertDialog.HideLoading();
        }
        catch (Exception ex)
        {
            alertDialog.HideLoading();
            Debug.LogError($"Error adding event: {ex.Message}");
        }
    }



    private void LoadData()
    {
        DocumentReference docRef = db.Collection(firestoreCollectionName).Document(docId);
        alertDialog.ShowLoading();
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {

                Debug.Log($"Document data for {snapshot.Id} document:");
                Timestamp startDateTimestamp = snapshot.GetValue<Timestamp>("StartDate");
                Timestamp endDateTimestamp = snapshot.GetValue<Timestamp>("EndDate");
                DateTime startDate = startDateTimestamp.ToDateTime();
                DateTime endDate = endDateTimestamp.ToDateTime();
                DisplayDateData(startDate,endDate);

                // Map data from the snapshot to the Restaurant object
                Name.text = snapshot.GetValue<string>("Name");
                Description.text = snapshot.GetValue<string>("Description");
                Location.text = snapshot.GetValue<string>("Location");
                Audience.text = snapshot.GetValue<string>("Audience");
                
                string priceString = snapshot.GetValue<string>("Price");
                double price;
                if (priceString.EndsWith("SAR")){
                 priceString = priceString.Substring(0, priceString.Length - 3); // Remove "SAR" suffix
                  }
             double.TryParse(priceString, out price);
             Price.text = price.ToString();

                string[] pictures = snapshot.GetValue<string[]>("Picture");
                string[] times = snapshot.GetValue<string>("WorkingHours").Split('-');
                startTime = times[0];
                endTime = times[1];
                // Splitting the start time string by ':' to get hours and minutes
                string[] startParts = startTime.Split(':');
                StartHour.value = int.Parse(startParts[0]);

                // Splitting the second part of start time string by ' ' to get minutes and AM/PM
                string[] minAmPmParts = startParts[1].Split(' ');
                StartMinute.value = int.Parse(minAmPmParts[0]) + 1;
                StartAmPm.value = minAmPmParts[1] == "AM" ? 0 : 1;

                // Splitting the end time string by ':' to get hours and minutes
                string[] endParts = endTime.Split(':');
                EndHour.value = int.Parse(endParts[0]);

                // Splitting the second part of end time string by ' ' to get minutes and AM/PM
                minAmPmParts = endParts[1].Split(' ');
                EndMinute.value = int.Parse(minAmPmParts[0]) + 1;
                EndAmPm.value = minAmPmParts[1] == "AM" ? 0 : 1; ;
                gallerySelection.DisplayLoadedImages(pictures.ToList<string>(), storageFolderName);
                alertDialog.HideLoading();
            }
            else
            {
                Debug.Log($"Document {snapshot.Id} does not exist!");
                alertDialog.HideLoading();
            }
        });
    }




}


