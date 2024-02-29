using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfilePageScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSceneToEditProfile()
    {
        StartCoroutine(LoadSceneEditProfile());
    }

    public IEnumerator LoadSceneEditProfile()
    {
        Debug.Log("IENUMERATOR changing scene to edit profile");
        var loadscene = SceneManager.LoadSceneAsync("EditProfile");
        while (!loadscene.isDone)
        {
            Debug.Log("loading the scene...");
            yield return null;
        }
        Debug.Log("after loading scene");

    }

}
