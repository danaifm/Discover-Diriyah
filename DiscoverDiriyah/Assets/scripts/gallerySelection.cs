using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryImageSelector : MonoBehaviour
{
    public RawImage[] imageDisplays; // Assign your 10 RawImage UI components in the inspector
    private List<string> selectedImagePaths = new List<string>(); // To store paths of selected images

    // Start is called before the first frame update
    void Start()
    {
        // Optionally, start the gallery access here or trigger it with a UI button
    }

    // Call this method when you want to start the image selection process
    public void StartImageSelectionProcess()
    {
        StartCoroutine(CheckAndRequestPermissionCoroutine());
    }

    IEnumerator CheckAndRequestPermissionCoroutine()
    {
        // Assuming you want to read images from the gallery
        NativeGallery.PermissionType permissionType = NativeGallery.PermissionType.Read;
        NativeGallery.MediaType mediaType = NativeGallery.MediaType.Image; // Assuming we're dealing with images

        // Check the current permission status for reading images
        NativeGallery.Permission permissionCheck = NativeGallery.CheckPermission(permissionType, mediaType);
        if (permissionCheck == NativeGallery.Permission.ShouldAsk)
        {
            // Request permission to read images
            NativeGallery.Permission permissionResult = NativeGallery.RequestPermission(permissionType, mediaType);
            yield return new WaitUntil(() => NativeGallery.CheckPermission(permissionType, mediaType) != NativeGallery.Permission.ShouldAsk);

            // Re-check the permission status after the user has responded to the permission request
            permissionCheck = NativeGallery.CheckPermission(permissionType, mediaType);
        }

        if (permissionCheck == NativeGallery.Permission.Granted)
        {
            // Permission is granted or was already granted
            PickImageFromGallery();
        }
        else
        {
            Debug.Log("Permission to access gallery denied");
        }
    }


    void PickImageFromGallery()
    {
        // Adjusted to call GetImageFromGallery repeatedly until the desired number of images is selected
        StartCoroutine(PickSingleImage());
    }

    IEnumerator PickSingleImage()
    {
        int maxImageCount = 10;
        while (selectedImagePaths.Count < maxImageCount)
        {
            bool isPickingImage = true;

            NativeGallery.GetImageFromGallery((path) =>
            {
                if (!string.IsNullOrEmpty(path))
                {
                    selectedImagePaths.Add(path);
                    StartCoroutine(DisplaySelectedImages(new string[] { path }));
                }
                isPickingImage = false;
            }, "Select an image", "image/*");

            // Wait until the image is picked
            yield return new WaitUntil(() => isPickingImage == false);

            // Optional: Check if the user wants to continue picking images or stop
            // This could be implemented as a UI prompt asking if the user wants to select more images
        }
    }

    IEnumerator DisplaySelectedImages(string[] paths)
    {
        foreach (string path in paths)
        {
            if (selectedImagePaths.Count > imageDisplays.Length)
            {
                Debug.LogWarning("Trying to display more images than the available RawImage components.");
                break;
            }

            int index = selectedImagePaths.IndexOf(path);
            Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024, false);
            if (texture != null && index < imageDisplays.Length)
            {
                imageDisplays[index].texture = texture;
                imageDisplays[index].gameObject.SetActive(true);
            }
            yield return null; // Ensure the UI updates for each image
        }

        // Optionally, disable any unused RawImage components if less than the intended images were selected
        for (int i = selectedImagePaths.Count; i < imageDisplays.Length; i++)
        {
            imageDisplays[i].gameObject.SetActive(false);
        }
    }

    // Method to retrieve selected image paths, for later use such as uploading to Firebase
    public List<string> GetSelectedImagePaths()
    {
        return selectedImagePaths;
    }
}
