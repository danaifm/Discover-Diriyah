using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{

    public static ScenesManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public enum Scene
    {
        userDiscoverpage,
        Map,
        ScanQR,
        Profile
    }

    public void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
        
    }

    public void LoadDicoverPage()
    {
        SceneManager.LoadScene(Scene.userDiscoverpage.ToString());
    }

    public void LoadMapPage()
    {
        SceneManager.LoadScene(Scene.Map.ToString());
    }

    public void LoadQRPage()
    {
        SceneManager.LoadScene(Scene.ScanQR.ToString());
    }

    public void LoadProfilePage()
    {
        SceneManager.LoadScene(Scene.Profile.ToString());
    }


    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

   
}
