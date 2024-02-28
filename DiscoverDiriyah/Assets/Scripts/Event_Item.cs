using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Storage;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase.Extensions;
using System;
using System.IO;

public class Event_Item : MonoBehaviour
{
    public GameObject FavouriteImage;
    public Image EventImage;
    public Sprite DefaultSprite;
    public Text TitleName;
    public Text DateText;

    private string localURL;
    FirebaseStorage storage;
    StorageReference storageRef;
    EventRoot event_Root;

    public void Init(EventRoot eventRoot)
    {
        event_Root = eventRoot;
        TitleName.text = eventRoot.Name;
        DateTime dateTime = DateTime.Parse(eventRoot.StartDate);
        DateText.text = dateTime.Day+"/"+ dateTime.Month+"/"+ dateTime.Year;
        CheckImage(event_Root.Picture[0]);
    }
    public void ShowAttractionDetails()
    {
        //DescriptionImagesManager.Instance.ShowDescription(event_Root);
    }
    public void CheckImage(string name)
    {
        ResetImage();
        DownloadImage(name);
        //Debug.Log("id and URL " + name);
        //localURL = string.Format("{0}/{1}.jpg", Application.persistentDataPath, "" + name);

        //if (File.Exists(localURL))
        //{
        //    Debug.Log("Image exist " + name);
        //    LoadLocalFile();
        //    //ConsoleManager.instance.ShowMessage("Image Found");
        //}
        //else
        //{
        //    DownloadImage(name);
        //}
    }
    public void LoadLocalFile()
    {
        byte[] bytes;
        bytes = File.ReadAllBytes(localURL);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        Sprite thumbnail = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        EventImage.sprite = thumbnail;
    }
    public void ResetImage()
    {
        EventImage.sprite = DefaultSprite;
    }
    public void DownloadImage(string name)
    {
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://discover-diriyah-96e5d.appspot.com/events");
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
            //rawImage.texture = texture;
            EventImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            //File.WriteAllBytes(localURL, texture.EncodeToPNG());
            //Debug.Log("Image Downloaded and saved at " + localURL);
        }
    }
}
