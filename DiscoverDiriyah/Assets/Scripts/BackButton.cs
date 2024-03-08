using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    AlertDialog alertDialog;
    private void Start()
    {
        alertDialog = FindObjectOfType<AlertDialog>();
    }
    public void LoadScene(string sceneName)
    {
        alertDialog.ShowConfirmDialog("Are you sure you want to go back?", (confirmed) =>
        {
            if (confirmed)
            {
                Debug.Log("User clicked Yes.");
                // Do something when user clicks Yes
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.Log("User clicked No.");
                // Do something when user clicks No
            }
        });
    }
}
