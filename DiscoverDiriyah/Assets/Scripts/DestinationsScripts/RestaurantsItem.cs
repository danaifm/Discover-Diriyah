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

public class RestaurantsItem : MonoBehaviour
{
    public GameObject FavouriteImage;
    public GameObject FavouriteDefaultImage;
    public Image RestaurantImage;
    public Sprite DefaultSprite;
    public Text TitleName;
    public Text CuisineTypeText;

    private string localURL;
    FirebaseStorage storage;
    StorageReference storageRef;
    RestaurantsRoot Restaurant_Root;

    private toggleFavorite toggleFav;

    public void Init(RestaurantsRoot restaurantsRoot)
    {
        if (!AdminFunctionalityManager.Admin)
        {
            if (restaurantsRoot.userFavorite)
                FavouriteImage.SetActive(true);
            else
                FavouriteDefaultImage.SetActive(true);
        }
        else
        {
            FavouriteDefaultImage.SetActive(false);
        }
        //FavouriteDefaultImage.SetActive(!AdminFunctionalityManager.Admin);
        Restaurant_Root = restaurantsRoot;
        TitleName.text = restaurantsRoot.Name;
        CuisineTypeText.text = restaurantsRoot.CuisineType;
        CheckImage(restaurantsRoot.Picture[0]);
    }
    public void ShowRestaurantDetails()
    {
        RestaurantDescriptionImagesManager.Instance.ShowDescription(Restaurant_Root, this);
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
        RestaurantImage.sprite = thumbnail;
    }
    public void ResetImage()
    {
        RestaurantImage.sprite = DefaultSprite;
    }
    public void DownloadImage(string name)
    {
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://discover-diriyah-96e5d.appspot.com/restaurant");
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
            RestaurantImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            //File.WriteAllBytes(localURL, texture.EncodeToPNG());
            //Debug.Log("Image Downloaded and saved at " + localURL);
        }

    }

    public void Favorite()
    {
        toggleFav = gameObject.AddComponent<toggleFavorite>();
        toggleFav.addToFavorites(Restaurant_Root.ID, "Restaurant");
        FavouriteImage.SetActive(true);
        Restaurant_Root.userFavorite = true;
    }

    public void Unfavorite()
    {
        toggleFav = gameObject.AddComponent<toggleFavorite>();
        toggleFav.removeFromFavorites(Restaurant_Root.ID);
        FavouriteImage.SetActive(false);
        FavouriteDefaultImage.SetActive(true);
        Restaurant_Root.userFavorite = false;
    }
}
