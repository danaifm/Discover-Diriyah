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

public class AddEvent : MonoBehaviour
{

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

    FirebaseFirestore db;
    FirebaseStorage storage;
    StorageReference storageRef;
    Dictionary<string, object> Event;
    public gallerySelection gallerySelection; 
    List<string> pictures;




    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);


        //DISABLE DATES FROM PAST 6 MONTHS
        if (endDatePicker != null)
        {
            endDatePicker.Content.OnDisplayChanged.AddListener(() => OnDisplayChanged(endDatePicker));
        }
        if (startDatePicker != null)
        {
            startDatePicker.Content.OnDisplayChanged.AddListener(() => OnDisplayChanged(startDatePicker));
        }

        pictures = gallerySelection.GetSelectedImagePaths();

    }



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

    

    public void validate_input()
    {
        bool isValid = true;

        //NAME FIELD VALIDATION
        name = Name.text.Trim();
        string pattern1 = @"^[a-zA-Z0-9 \-\[\]\(\),]*$";
        if (!Regex.IsMatch(name, pattern1) || string.IsNullOrWhiteSpace(name))
        {
            nameError.text = "Invalid name";
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
        string pattern2 = @"^[a-zA-Z0-9 \-\[\],:;?!().]*$";
        if (!Regex.IsMatch(description, pattern2) || string.IsNullOrWhiteSpace(description))
        {
            descriptionError.text = "Invalid description";
            descriptionError.color = Color.red;
            descriptionError.fontSize = 30;
            isValid = false;

        }
        else
        {  
            descriptionError.text = "";
        }

        //AUDIENCE FIELD VALIDATION
        audience = Audience.text.Trim();
        string pattern3 = @"^[a-zA-Z0-9 \-\+\(\)]*$";
        if (!Regex.IsMatch(audience, pattern3) || string.IsNullOrWhiteSpace(audience))
        {
            audienceError.text = "Invalid audience";
            audienceError.color = Color.red;
            audienceError.fontSize = 30;
            isValid = false;

        }
        else
        {
            audienceError.text = "";
        }

        //LOCATION FIELD VALIDATION
        location = Location.text.Trim();
        string urlPattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";

        if (!Regex.IsMatch(location, urlPattern) || string.IsNullOrWhiteSpace(location))
        {
            locationError.text = "Invalid location URL";
            locationError.color = Color.red;
            locationError.fontSize = 30;
            isValid = false;

        }
        else
        {
            locationError.text = "";
        }

        //START TIME VALIDATION
        if(StartHour.value == 0 && StartMinute.value == 0)
        {
            startTimeError.text = "Start hour and minute must be selected";
            startTimeError.color = Color.red;
            startTimeError.fontSize = 30;
            isValid = false;

        }
        
       
        else if (StartHour.value == 0)
        {
            startTimeError.text = "Start hour must be selected";
            startTimeError.color = Color.red;
            startTimeError.fontSize = 30;
            isValid = false;

        }
        

        else if (StartMinute.value == 0)
        {
            startTimeError.text = "Start minute must be selected";
            startTimeError.color = Color.red;
            startTimeError.fontSize = 30;
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
            endTimeError.fontSize = 30;
            isValid = false;

        }
        
        else if (EndHour.value == 0)
        {
            endTimeError.text = "End hour must be selected";
            endTimeError.color = Color.red;
            endTimeError.fontSize = 30;
            isValid = false;

        }
        

        else if (EndMinute.value == 0)
        {
            endTimeError.text = "End minute must be selected";
            endTimeError.color = Color.red;
            endTimeError.fontSize = 30;
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
            if (selection >= currentDate)
            {
                startDateError.text = "";
            }
            else //date from past 
            {
                startDateError.text = "Start date cannot be in the past.";
                startDateError.color = Color.red;
                startDateError.fontSize = 30;
                isValid = false;
            }
        }
        else //field left empty
        {
            startDateError.text = "Start date must be selected.";
            startDateError.color = Color.red;
            startDateError.fontSize = 30;
            isValid = false;
        }

        // END DATE VALIDATION 
        DateTime? selectedEndDate = null; 
        if (endDatePicker != null && endDatePicker.Content.Selection != null && endDatePicker.Content.Selection.Count > 0)
        {
            var selection = endDatePicker.Content.Selection.GetItem(0); //returns a DateTime
            selectedEndDate = selection;
            if (selection >= currentDate)
            {
                endDateError.text = "";
            }
            else //past date
            {
                endDateError.text = "End date cannot be in the past.";
                endDateError.color = Color.red;
                endDateError.fontSize = 30;
                isValid = false;
            }
        }
        else //field left empty
        {
            endDateError.text = "End date must be selected.";
            endDateError.color = Color.red;
            endDateError.fontSize = 30;
            isValid = false;
        }

        // validation for end date not before start date
        if (selectedStartDate.HasValue && selectedEndDate.HasValue)
        {
            if (selectedEndDate.Value < selectedStartDate.Value)
            {
                endDateError.text = "End date cannot be before start date.";
                endDateError.color = Color.red;
                endDateError.fontSize = 30;
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
        if (!Regex.IsMatch(price, pricePattern) || string.IsNullOrWhiteSpace(price))
        {
            priceError.text = "Invalid price";
            priceError.color = Color.red;
            priceError.fontSize = 30;
            isValid = false;

        }
        else
        {
            priceError.text = "";
            price += " SAR";
        }

        //GETTING ARRAY OF PICTURES 
        

        //if everything is valid -> upload to firebase 
        if (isValid)
        {
            uploadEvent();
        }

    }//end of validations 

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
        List<string> uploadedImageNames = await UploadImages(pictures,name); // Call your UploadImages method

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
        {"Picture", uploadedImageNames ?? new List<string>()} // Use null-coalescing operator to handle null
    };

        try
        {
            // Assuming 'db' is already initialized Firestore instance and ready to use
            var docRef = await db.Collection("Event").AddAsync(newEvent);
            Debug.Log($"Event added successfully with ID: {docRef.Id}");

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
            Debug.LogError($"Error adding event: {ex.Message}");
        }
    }

}


