using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using Bitsplash.DatePicker;
using UI.Dates;
using UnityEditor;
using System;

public class AddEvent : MonoBehaviour
{
    //void Start() {     SetToCurrentMonthAndYear();}

public TMP_InputField Name;
    public TMP_Text nameError;
    public TMP_InputField Description;
    public TMP_Text descriptionError;
    public TMP_InputField Audience;
    public TMP_Text audienceError;
    public TMP_Dropdown StartHour;
    public TMP_Dropdown StartMinute;
    public TMP_Text startTimeError;
    public TMP_Dropdown EndHour;
    public TMP_Dropdown EndMinute;
    public TMP_Text endTimeError;
    public TMP_InputField Location;
    public TMP_Text locationError;
    public TMP_Text startDateError;
    public TMP_Text endDateError;
    public DatePickerSettings startDatePicker;
    public DatePickerSettings endDatePicker;
    public TMP_InputField Price;
    public TMP_Text priceError;



    void Start()
    {
        //DISABLE DATES FROM PAST 6 MONTHS
        if (endDatePicker != null)
        {
            endDatePicker.Content.OnDisplayChanged.AddListener(() => OnDisplayChanged(endDatePicker));
        }
        if (startDatePicker != null)
        {
            startDatePicker.Content.OnDisplayChanged.AddListener(() => OnDisplayChanged(startDatePicker));
        }
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
        string name = Name.text;
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
        string  description = Description.text;
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
        string audience = Audience.text;
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
        string location = Location.text;
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
        { startTimeError.text = ""; }

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
        { endTimeError.text = ""; }


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
        }

        //PRICE FIELD VALIDATION
        string price = Price.text;
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
        }

        //if everything is valid -> upload to firebase 
        if (isValid)
        {
            uploadEvent();
        }

    }//end of validations 

    public void uploadEvent()
    {

    }

}
