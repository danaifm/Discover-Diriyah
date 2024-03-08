using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlertDialog : MonoBehaviour
{
    public TMP_Text messageText;
    public string sceneLoaded;
    public GameObject alertObject;
    public void ShowAlertDialog(string message)
    {
        messageText.text = message;
        alertObject.SetActive(true);
    }
    public void OnClickOkButton()
    {
        alertObject.SetActive(false);
        SceneManager.LoadScene(sceneLoaded);
    }
}
