using Firebase.Extensions;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class gallerySelection : MonoBehaviour
{
    public RawImage[] imageDisplays; // Assign your 10 RawImage UI components in the inspector
    private List<string> selectedImagePaths = new List<string>(); // To store paths of selected images
    public Button selectImageButton; // Assign your button for selecting images
    public Texture defaultTexture;
    public TextAsset defaultImageTexture;
    StorageReference storageRef;
    FirebaseStorage storage;
    void Awake()
    {
        storage = FirebaseStorage.DefaultInstance;
        string storageUrl = "gs://discover-diriyah-96e5d.appspot.com";
        storageRef = storage.GetReferenceFromUrl(storageUrl);
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
                StartCoroutine(DisplaySelectedImage(path));
                UpdateButtonInteractivity(); // Update button interactivity based on the new count of selected images
            }
        }, "Select an image", "image/*");
    }

    public void DisplayLoadedImages(List<string> images)
    {
        selectedImagePaths = images;
        DisplayImages();
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

    public void RemoveImage(int index)
    {
        try
        {
            imageDisplays[index].texture = null;
            imageDisplays[selectedImagePaths.Count - 1].texture = null;
            //-- remove it from storage
            if (!File.Exists(selectedImagePaths[index]))
            {
                imageDisplays[index].texture = null;
                RemoveImageFromFirebaseStorage(index);
                return;
            }
            selectedImagePaths.RemoveAt(index);
            DisplayImages();

        }
        catch
        {
            Debug.Log("index " + index + " not exist");
        }
    }
    public void DisplayImages()
    {
        for (int i = 0; i < imageDisplays.Length; i++)
        {
            if (selectedImagePaths.Count > i)
            {
                string imagePath = selectedImagePaths[i];

                // Check if the image path is a local path or a Firebase Storage URL
                if (File.Exists(imagePath))
                {
                    // Local path obtained from gallery
                    Texture2D texture = NativeGallery.LoadImageAtPath(imagePath, 1024, false, false);
                    imageDisplays[i].texture = texture;
                    imageDisplays[i].color = new Color(imageDisplays[i].color.r, imageDisplays[i].color.g, imageDisplays[i].color.b, 1);
                }
                else
                {
                    try
                    {
                        // Firebase Storage URL
                        LoadImageFromFirebaseStorage(imagePath, i);
                    }
                    catch (Exception e)
                    {
                        print(e.Message);
                    }
                }
            }
            else
            {
                imageDisplays[i].texture = defaultTexture;
                imageDisplays[i].color = new Color(imageDisplays[i].color.r, imageDisplays[i].color.g, imageDisplays[i].color.b, 1); // Alpha set to 1
            }
        }
    }

    private void LoadImageFromFirebaseStorage(string fileName, int index)
    {
        StorageReference fileRef = storageRef.Child("accommodations").Child(fileName);
        const long maxAllowedSize = 10 * 1024 * 1024;
        print("Load from firebase");
        fileRef.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception);
                selectedImagePaths.RemoveAt(index);
                // Uh-oh, an error occurred!
            }
            else
            {
                byte[] fileContents = task.Result;

                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(fileContents);
                texture.Apply(); // Apply changes to the texture
                print("Apply done.");
                imageDisplays[index].texture = texture;
                imageDisplays[index].color = new Color(imageDisplays[index].color.r, imageDisplays[index].color.g, imageDisplays[index].color.b, 1); // Alpha set to 0
                Debug.Log("Finished downloading!");
            }
        });

        
    }


    private void RemoveImageFromFirebaseStorage(int index)
    {
        StorageReference fileRef = storageRef.Child("restaurant").Child(selectedImagePaths[index]);

        fileRef.DeleteAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError(task.Exception != null ? task.Exception.ToString() : "Delete image task was cancelled.");
                //-- sometimes images not exist in storage.
                //selectedImagePaths.RemoveAt(index);
            }
            else
            {
                Debug.Log("Image deleted successfully from Firebase Storage." + selectedImagePaths[index]);
                selectedImagePaths.RemoveAt(index);
                DisplayImages();
            }
        });
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








/*using System.Collections;
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
}*/
