using System.Collections;
using UnityEngine;
using TMPro;

public class manageTime : MonoBehaviour
{
    public TMP_Dropdown hourDropdown;
    public TMP_Dropdown minuteDropdown;
    private int lastHourSelection = 0;
    private int lastMinuteSelection = 0;

    void Start()
    {
        // Initialize dropdowns with your values including "HH" and "MM"

        // Add listener for each dropdown to handle user selection
        hourDropdown.onValueChanged.AddListener(delegate { HandleHourSelection(); });
        minuteDropdown.onValueChanged.AddListener(delegate { HandleMinuteSelection(); });
    }

    void HandleHourSelection()
    {
        // Check if "HH" is selected
        if (hourDropdown.value == 0) // Assuming "HH" is at index 0
        {
            // Revert to last valid selection or default
            hourDropdown.value = lastHourSelection > 0 ? lastHourSelection : 1; // Change 1 to a default valid selection if needed
        }
        else
        {
            // Update last valid selection
            lastHourSelection = hourDropdown.value;
        }
    }

    void HandleMinuteSelection()
    {
        // Check if "MM" is selected
        if (minuteDropdown.value == 0) // Assuming "MM" is at index 0
        {
            // Revert to last valid selection or default
            minuteDropdown.value = lastMinuteSelection > 0 ? lastMinuteSelection : 1; // Change 1 to a default valid selection if needed
        }
        else
        {
            // Update last valid selection
            lastMinuteSelection = minuteDropdown.value;
        }
    }
}
