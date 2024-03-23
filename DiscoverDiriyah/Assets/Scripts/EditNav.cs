using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditNav : MonoBehaviour
{

    public static EditNav Instance; 
    private void Awake()
    {
        Instance = this;
    }

    public enum Scene
    {
        
        EditAccommodation,
        EditRestaurant,
        EditAttraction,
        EditEvent,
    }

    public void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());

    }

    

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
