using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Storage;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase.Extensions;
using System;

public class FirebaseStorageManager : MonoBehaviour
{
    public string ImageUrl;
    FirebaseStorage storage;
    StorageReference storageRef;
    public RawImage rawImage;

    void Start()
    {
        // Initialize Firebase Storage
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase is ready to use
                storage = FirebaseStorage.DefaultInstance;
                storageRef = storage.GetReferenceFromUrl("gs://diriyah-300d5.appspot.com");
                Debug.Log("Firebase Storage initialized successfully!");
                DownloadUserImage();
                // Call method to download image
                //DownloadImage("Screenshot 2024-02-12 at 6.19.09 PM.png");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
        //StartCoroutine(GetThumbnail(ImageUrl));
    }
    public void DownloadUserImage()
    {
        StorageReference image = storageRef.Child("Screenshot 2024-02-12 at 6.19.09 PM.png");

        //Get the download link of file
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Image url" + (task.Result).ToString());
                StartCoroutine(GetThumbnail(Convert.ToString(task.Result)));
                //StartCoroutine(LoadImage(Convert.ToString(task.Result))); //Fetch file from the link
            }
            else
            {
                Debug.Log(task.Exception);
            }
        });
    }
    IEnumerator GetThumbnail(string uri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
        //www.SetRequestHeader("Content-type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Image not Downloaded");
            Debug.Log(www.responseCode);
        }
        else
        {
            Debug.Log("Image Downloaded");
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            rawImage.texture = texture;
            //ProfileImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            //File.WriteAllBytes(localURL, texture.EncodeToPNG());

        }
    }
}
