using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestaurantDescriptionImagesManager : MonoBehaviour
{
    public static RestaurantDescriptionImagesManager Instance;

    public GameObject DescriptionPanel;
    public Image DescriptionImage;
    public Text PlaceTitle;
    public Text CuisineType;

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
    public void ShowDescription(RestaurantsRoot restaurantsRoot)
    {
        DescriptionPanel.SetActive(true);
        PlaceTitle.text = restaurantsRoot.Name;
        CuisineType.text = restaurantsRoot.CuisineType;

        if (restaurantsRoot.userFavorite)
        {
            FavouriteImage.SetActive(true);
        }
        else
        {
            FavouriteImage.SetActive(false);
            FavouriteDefaultImage.SetActive(true);
        }

        //Description.text = attractionsRoot.Description;
        LocationUrl = restaurantsRoot.Location;
        GameObject temp;
        for (int i = 0; i < restaurantsRoot.Picture.Count; i++)
        {
            temp = Instantiate(UI_Prefab, ParentContent);
            temp.GetComponent<RestaurantDescriptionImageItem>().Init(restaurantsRoot.Picture[i]);
        }
        currentIndex = 0;
        GetAllChildGameObjects();
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
            child.gameObject.GetComponent<RestaurantDescriptionImageItem>().ItemIndex = ChildIndex;
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
        DesImageItems[index].GetComponent<RestaurantDescriptionImageItem>().OnSelect();
    }

    private void DeactivateOtherItems(int index)
    {
        DesImageItems[index].GetComponent<RestaurantDescriptionImageItem>().OnPrevious();
    }

}
