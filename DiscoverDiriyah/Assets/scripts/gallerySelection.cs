using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryImageSelector : MonoBehaviour
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

        // Optionally, disable the button if you reach the maximum number of images (10 in this case)
        UpdateButtonInteractivity();
    }

    // Linked to the button's onClick event
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
            if (!string.IsNullOrEmpty(path))
            {
                selectedImagePaths.Add(path);
                StartCoroutine(DisplaySelectedImages(new string[] { path }));
                UpdateButtonInteractivity(); // Update button interactivity based on the new count of selected images
            }
        }, "Select an image", "image/*");
    }

    IEnumerator DisplaySelectedImages(string[] paths)
    {
        foreach (string path in paths)
        {
            int index = selectedImagePaths.IndexOf(path);
            if (index < imageDisplays.Length)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024, false, false); // Set markTextureNonReadable to false
                if (texture != null)
                {
                    imageDisplays[index].texture = texture;
                    // Make the RawImage component opaque to show the image
                    imageDisplays[index].color = new Color(imageDisplays[index].color.r, imageDisplays[index].color.g, imageDisplays[index].color.b, 1); // Alpha set to 1
                    Debug.Log("image saved");
                }
            }
            yield return null; // Ensure the UI updates for each image
        }
    }

    void UpdateButtonInteractivity()
    {
        // Disable the button if the maximum number of images has been selected
        selectImageButton.interactable = selectedImagePaths.Count < 10;
    }

    // Method to retrieve selected image paths, for later use such as uploading to Firebase
    public List<string> GetSelectedImagePaths()
    {
        return selectedImagePaths;
    }
}
