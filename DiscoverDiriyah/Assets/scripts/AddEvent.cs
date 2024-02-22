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



    void Start() {
        if (endDatePicker != null)
        {
            endDatePicker.Content.OnDisplayChanged.AddListener(OnDisplayChanged);

        }
    }

    public void OnDisplayChanged()
    {
        var cell = endDatePicker.Content.GetCellObjectByDate(DateTime.Now);
        if (cell != null)
        {
            cell.CellEnabled = false;
        }
    }


    public void validate_input()
    {
        bool isValid = true;

        //name field validation
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
        
        //description field validation
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

        //audience field validation
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

        //location field validation
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

        //start time validation 
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

        //End time validation 
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

        //start date validation
        if (startDatePicker != null)
        {
            if (startDatePicker.Content.Selection != null && startDatePicker.Content.Selection.Count > 0)
            {
                var selection = startDatePicker.Content.Selection.GetItem(0);
                string selectedStartDate = selection.ToString()[..9];
                startDateError.text = "";


            }

            else
            {
                startDateError.text = "Start date must be selected.";
                startDateError.color = Color.red;
                startDateError.fontSize = 30;
            }
        }
        else
        {
            startDateError.text = "Start date must be selected.";
            startDateError.color = Color.red;
            startDateError.fontSize = 30;
        }

        //end date validation
        if (endDatePicker != null)
        {

            if (endDatePicker.Content.Selection != null && endDatePicker.Content.Selection.Count > 0)
            {
                var selection = endDatePicker.Content.Selection.GetItem(0);
                string selectedEndDate = selection.ToString()[..9];
                endDateError.text = "";


            }

            else
            {
                endDateError.text = "End date must be selected.";
                endDateError.color = Color.red;
                endDateError.fontSize = 30;
            }
        }
        else
        {
            endDateError.text = "End date must be selected.";
            endDateError.color = Color.red;
            endDateError.fontSize = 30;
        }

        //price field validation
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


    }



}
