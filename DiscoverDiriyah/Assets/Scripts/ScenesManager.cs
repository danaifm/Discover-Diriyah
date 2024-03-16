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
        ProfilePage,
        AddAttraction,
        AddRestaurant,
        AddEventDetails,
        AddAccommodation,
        EditAccommodation,
        EditRestaurant, 
        EditAttraction,
        EditEvent,
        EditProfile
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
        SceneManager.LoadScene(Scene.ProfilePage.ToString());
    }


    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadAddAttractionsPage()
    {
        SceneManager.LoadScene(Scene.AddAttraction.ToString(), LoadSceneMode.Additive);
    }

    public void LoadAddAccommodationsPage()
    {
        SceneManager.LoadScene(Scene.AddAccommodation.ToString(), LoadSceneMode.Additive);
    }

    public void LoadAddRestaurantsPage()
    {
        SceneManager.LoadScene(Scene.AddRestaurant.ToString(), LoadSceneMode.Additive);
    }


    public void LoadAddEventsPage()
    {
        SceneManager.LoadScene(Scene.AddEventDetails.ToString(), LoadSceneMode.Additive);
    }

    public void LoadEditAttractionsPage()
    {
        SceneManager.LoadScene(Scene.EditAttraction.ToString(), LoadSceneMode.Additive);
    }

    public void LoadEditAccommodationsPage()
    {
        SceneManager.LoadScene(Scene.EditAccommodation.ToString(), LoadSceneMode.Additive);
    }

    public void LoadEditRestaurantsPage()
    {
        SceneManager.LoadScene(Scene.EditRestaurant.ToString(), LoadSceneMode.Additive);
    }

    public void LoadEditEventsPage()
    {
        SceneManager.LoadScene(Scene.EditEvent.ToString(), LoadSceneMode.Additive);
    }


}
