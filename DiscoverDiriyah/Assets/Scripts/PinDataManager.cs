using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PinDataManager : MonoBehaviour
{
    public static PinDataManager instance;

    public Text Title;
    public Text Category;
    public Image LocationImage;
    public Sprite DefaultSprite;
    public Animator animator;


    FirebaseStorage storage;
    StorageReference storageRef;
    private string LocationUrl;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        
    }
    public void DisplayPinData(LocationsData locationsData)
    {
        Title.text = locationsData.Title;
        Category.text = locationsData.Catagory;
        LocationUrl = locationsData.Location;
        DisplayLocationPopup();
        //CheckImage("1.jpeg");
        CheckImage(locationsData.Picture, locationsData.Catagory);
    }
    public void OpenLocationUrl()
    {
        if (LocationUrl.Length>0)
        {
            Application.OpenURL(LocationUrl);
        }
    }
    private void DisplayLocationPopup()
    {
        ResetAnimTriggers();
        animator.SetTrigger("ShowMapPin");
    }
    public void HideLocationPopup()
    {
        ResetAnimTriggers();
        animator.SetTrigger("CloseMapPin");
    }
    private void ResetAnimTriggers()
    {
        animator.ResetTrigger("ShowMapPin");
        animator.ResetTrigger("CloseMapPin");
    }
    private void CheckImage(string name, string CategoryName)
    {
        ResetImage();
        DownloadImage(name,CategoryName);
    }
    public void ResetImage()
    {
        LocationImage.sprite = DefaultSprite;
    }
    public void DownloadImage(string name, string CategoryName)
    {
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://discover-diriyah-96e5d.appspot.com/"+ FindCategoryDB_Name(CategoryName));
        StorageReference image = storageRef.Child(name);

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
    private string FindCategoryDB_Name(string CategoryName)
    {
        if (CategoryName == "Accommodation")
        {
            return "accommodations";
        }
        else if (CategoryName == "Attraction")
        {
            return "attractions";
        }
        else if (CategoryName == "Event")
        {
            return "events";
        }
        else if (CategoryName == "Restaurant")
        {
            return "restaurant";
        }
        else
        {
            return "accommodations";
        }
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

            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            LocationImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
