using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class gallerySelection : MonoBehaviour
{
    public RawImage[] imageDisplays; // Assign your 10 RawImage UI components in the inspector
    private List<string> selectedImagePaths = new List<string>(); // To store paths of selected images
    public Button selectImageButton; // Assign your button for selecting images

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
                StartCoroutine(DisplaySelectedImages(new string[] { path }));
                UpdateButtonInteractivity(); // Update button interactivity based on the new count of selected images
            }
        }, "Select an image", "image/*");
    }

    public void DisplayLoadedImages(List<string> images)
    {
        selectedImagePaths = images;
        DisplayImages();
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
    public void RemoveImage(int index)
    {
        try
        {
            if (selectedImagePaths[index] == null) return;

            //-- remove it from storage
            if (!File.Exists(selectedImagePaths[index]))
            {
                RemoveImageFromFirebaseStorage(index);
                return;
            }
            selectedImagePaths.RemoveAt(index);
            DisplayImages();
            /*for(int i = 0; i < imageDisplays.Length; i++)
            {
                if (selectedImagePaths.Count > i)
                {
                    Texture2D texture = NativeGallery.LoadImageAtPath(selectedImagePaths[i], 1024, false, false); // Set markTextureNonReadable to false
                    imageDisplays[i].texture = texture;
                    //imageDisplays[i].color = new Color(imageDisplays[index].color.r, imageDisplays[index].color.g, imageDisplays[index].color.b, 1); // Alpha set to 1
                }
                else
                {
                    imageDisplays[i].texture = null;
                }
            }*/
            
        }
        catch{
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
                        StartCoroutine(LoadImageFromFirebaseStorage(imagePath, i));
                    }catch(Exception e)
                    {
                        print(e.Message);
                    }
                }
            }
            else
            {
                imageDisplays[i].texture = null;
            }
        }
    }

    private IEnumerator LoadImageFromFirebaseStorage(string fileName, int index)
    {
            StorageReference fileRef = storageRef.Child("restaurant").Child(fileName);
            const long maxAllowedSize = 10 * 1024 * 1024;

            // Fetch image bytes asynchronously
            var task = fileRef.GetBytesAsync(maxAllowedSize);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError(task.Exception != null ? task.Exception.ToString() : "Load image task was cancelled.");
                yield break;
            }
        try
        {
            byte[] fileContents = task.Result;

            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(fileContents);
            texture.Apply(); // Apply changes to the texture

            imageDisplays[index].texture = texture;
            imageDisplays[index].color = new Color(imageDisplays[index].color.r, imageDisplays[index].color.g, imageDisplays[index].color.b, 1); // Alpha set to 1
            Debug.Log("Finished downloading!");
        }catch(Exception e)
        {
            print(e.Message);
            yield break;
        }
    }
    private void RemoveImageFromFirebaseStorage(int index)
    {
        StorageReference fileRef = storageRef.Child("restaurant").Child(selectedImagePaths[index]);

        fileRef.DeleteAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError(task.Exception != null ? task.Exception.ToString() : "Delete image task was cancelled.");
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
