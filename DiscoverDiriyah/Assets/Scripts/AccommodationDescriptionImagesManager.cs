using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccommodationDescriptionImagesManager : MonoBehaviour
{
    public static AccommodationDescriptionImagesManager Instance;

    public GameObject DescriptionPanel;
    public Image[] Stars;
    public Image DescriptionImage;
    public Text PlaceTitle;
    public Text Description;

    public Transform ParentContent;
    public GameObject UI_Prefab;

    private List<GameObject> DesImageItems = new List<GameObject>();
    private int currentIndex = 0;
    private string LocationUrl = "";

    public GameObject FavouriteImage;
    public GameObject FavouriteDefaultImage;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        //currentIndex = 0;
        //GetAllChildGameObjects();
    }
    public void ShowDescription(AccommodationRoot accommodationRoot)
    {
        DescriptionPanel.SetActive(true);
        PlaceTitle.text = accommodationRoot.Name;
        Description.text = accommodationRoot.Description;
        LocationUrl = accommodationRoot.Location;

        if (accommodationRoot.userFavorite)
        {
            FavouriteImage.SetActive(true);
        }
        else
        {
            FavouriteImage.SetActive(false);
            FavouriteDefaultImage.SetActive(true);
        }

        GameObject temp;
        SetRating(accommodationRoot.StarRating);
        for (int i = 0; i < accommodationRoot.Picture.Count; i++)
        {
            temp = Instantiate(UI_Prefab, ParentContent);
            temp.GetComponent<AccommodationDescriptionImageItem>().Init(accommodationRoot.Picture[i]);
        }
        currentIndex = 0;
        GetAllChildGameObjects();
    }
    public void SetRating(float rating)
    {
        int fullStars = Mathf.FloorToInt(rating); // Get the integer part of the rating
        float remainder = rating - fullStars; // Get the decimal part of the rating

        // Set full stars
        for (int i = 0; i < fullStars; i++)
        {
            Stars[i].fillAmount = 1f; // Fill the image fully
        }
        // Set half star if necessary
        if (fullStars < Stars.Length && remainder > 0)
        {
            Stars[fullStars].fillAmount = remainder; // Fill the next image partially
        }
        // Clear remaining stars
        for (int i = fullStars + 1; i < Stars.Length; i++)
        {
            Stars[i].fillAmount = 0f; // Clear the remaining images
        }
    }
    public void OpenLocationUrl()
    {
        Application.OpenURL(LocationUrl);
    }
    public void Back()
    {
        DescriptionPanel.SetActive(false);
        foreach (Transform child in ParentContent)
        {
            Destroy(child.gameObject);
        }
    }
    private void GetAllChildGameObjects()
    {
        // Clear the existing list
        DesImageItems.Clear();
        int ChildIndex = 0;
        // Iterate through all child GameObjects of the parent
        foreach (Transform child in ParentContent)
        {
            // Add the child GameObject to the list
            DesImageItems.Add(child.gameObject);
            child.gameObject.GetComponent<AccommodationDescriptionImageItem>().ItemIndex = ChildIndex;
            ChildIndex++;
        }
        if (DesImageItems.Count > 0)
        {
            // Activate the first game object in the list
            ActivateItem(currentIndex);
        }
    }
    public void ChangeDescriptionImage(Sprite NewSprite)
    {
        DescriptionImage.sprite = NewSprite;
    }
    public void OnSpecificItemSelection(int ItemIndex)
    {
        DeactivateOtherItems(currentIndex);
        // Update currentIndex to ItemIndex
        currentIndex = ItemIndex;

        // Activate the specific game object
        ActivateItem(currentIndex);
    }
    public void Next()
    {
        // Move to the next index
        int nextIndex = currentIndex + 1;

        // Check if nextIndex exceeds the list bounds
        if (nextIndex < DesImageItems.Count)
        {
            // Deactivate the current game object
            DeactivateOtherItems(currentIndex);

            // Update currentIndex to the next index
            currentIndex = nextIndex;

            // Activate the next game object
            ActivateItem(currentIndex);
        }
    }

    public void Previous()
    {
        // Move to the previous index
        int previousIndex = currentIndex - 1;

        // Check if previousIndex is within the list bounds
        if (previousIndex >= 0)
        {
            // Deactivate the current game object
            DeactivateOtherItems(currentIndex);

            // Update currentIndex to the previous index
            currentIndex = previousIndex;

            // Activate the previous game object
            ActivateItem(currentIndex);
        }
    }

    private void ActivateItem(int index)
    {
        DesImageItems[index].GetComponent<AccommodationDescriptionImageItem>().OnSelect();
    }

    private void DeactivateOtherItems(int index)
    {
        DesImageItems[index].GetComponent<AccommodationDescriptionImageItem>().OnPrevious();
    }

}
