using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gallerySelection : MonoBehaviour
{
    public RawImage[] imageDisplays; // Assign your 10 RawImage UI components in the inspector
    private List<string> selectedImagePaths = new List<string>(); // To store paths of selected images
    public Button selectImageButton; // Assign your button for selecting images

    void Start()
    {
        // Make all RawImage components fully transparent initially
        foreach (var imageDisplay in imageDisplays)
        {
            imageDisplay.color = new Color(imageDisplay.color.r, imageDisplay.color.g, imageDisplay.color.b, 0); // Alpha set to 0
        }

        UpdateButtonInteractivity();
    }

    public void OnSelectImageButtonClicked()
    {
        if (selectedImagePaths.Count < 10)
        {
            PickImageFromGallery();
        }
    }

    void PickImageFromGallery()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path) && selectedImagePaths.Count < 10)
            {
                selectedImagePaths.Add(path);
                StartCoroutine(DisplaySelectedImage(path));
                UpdateButtonInteractivity(); // Update button interactivity based on the new count of selected images
            }
        }, "Select an image", "image/*");
    }

    IEnumerator DisplaySelectedImage(string path)
    {
        // Use the count of selectedImagePaths to determine the display index
        int displayIndex = selectedImagePaths.Count - 1;
        if (displayIndex < imageDisplays.Length)
        {
            Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024, false, false); // Set markTextureNonReadable to false
            if (texture != null)
            {
                imageDisplays[displayIndex].texture = texture;
                imageDisplays[displayIndex].color = new Color(imageDisplays[displayIndex].color.r, imageDisplays[displayIndex].color.g, imageDisplays[displayIndex].color.b, 1); // Alpha set to 1
                Debug.Log("Image displayed at index: " + displayIndex);
            }
        }
        yield return null; // Ensure the UI updates for each image
    }

    void UpdateButtonInteractivity()
    {
        selectImageButton.interactable = selectedImagePaths.Count < 10;
    }

    public List<string> GetSelectedImagePaths()
    {
        return selectedImagePaths;
    }
}
