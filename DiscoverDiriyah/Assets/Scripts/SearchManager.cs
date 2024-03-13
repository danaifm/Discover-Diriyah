using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{
    [Header("Attractions Data")]
    public GameObject AttractionsContentParent;
    public InputField AttractionsSearchInputField;

    [Header("Events Data")]
    public GameObject EventsContentParent;
    public InputField EventsSearchInputField;

    [Header("Restaurants Data")]
    public GameObject RestaurantsContentParent;
    public InputField RestaurantsSearchInputField;

    [Header("Accommodations Data")]
    public GameObject AccommodationContentParent;
    public InputField AccommodationSearchInputField;

    private void Start()
    {
        // Add a listener to the input field's OnValueChanged event
        //searchInputField.onValueChanged.AddListener(delegate { OnSearchInputChanged(); });
    }
    public void SearchFromAttractions()
    {
        // Get the current text entered in the input field
        string searchText = AttractionsSearchInputField.text.ToLower();

        for (int i = 0; i < AttractionsManager.AttractionsData.Count; i++)
        {
            name = AttractionsManager.AttractionsData[i].Name;
            if (name.ToLower().Contains(searchText))
            {
                AttractionsContentParent.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                AttractionsContentParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void SearchFromEvents()
    {
        // Get the current text entered in the input field
        string searchText = EventsSearchInputField.text.ToLower();

        for (int i = 0; i < EventsManager.Instance.EventsData.Count; i++)
        {
            name = EventsManager.Instance.EventsData[i].Name;
            if (name.ToLower().Contains(searchText))
            {
                EventsContentParent.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                EventsContentParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void SearchFromRestaurants()
    {
        // Get the current text entered in the input field
        string searchText = RestaurantsSearchInputField.text.ToLower();

        for (int i = 0; i < RestaurantsManager.Instance.RestaurantsData.Count; i++)
        {
            name = RestaurantsManager.Instance.RestaurantsData[i].Name;
            if (name.ToLower().Contains(searchText))
            {
                RestaurantsContentParent.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                RestaurantsContentParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void SearchFromAccommodations()
    {
        // Get the current text entered in the input field
        string searchText = AccommodationSearchInputField.text.ToLower();

        for (int i = 0; i < AccommodationManager.Instance.AccommodationsData.Count; i++)
        {
            name = AccommodationManager.Instance.AccommodationsData[i].Name;
            if (name.ToLower().Contains(searchText))
            {
                AccommodationContentParent.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                AccommodationContentParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
