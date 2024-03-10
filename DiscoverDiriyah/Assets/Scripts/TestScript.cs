using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using System;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Networking;

public class TestScript : MonoBehaviour
{
    public string[] imageUrl; // URL of the image in Firebase Storage
    public Image imageReference; // Reference to the UI Image component where the downloaded image will be displayed
    public int index = 0;

    private void Start()
    {
        StartCoroutine(GetThumbnail(imageUrl[index]));
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
            imageReference.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            //File.WriteAllBytes(localURL, texture.EncodeToPNG());
            //Debug.Log("Image Downloaded and saved at " + localURL);
        }
    }
}
