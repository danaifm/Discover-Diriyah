using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AlertDialog : MonoBehaviour
{
    public TMP_Text alertMessage;
    public string sceneLoaded;
    public GameObject alertObject;
    public GameObject loadingObject;
    public TMP_Text confirmMessage;
    public GameObject confirmObject;
    public Button yesButton;
    public Button noButton;

    private System.Action<bool> callback;
    private int loadingRequestCount = 0;
    public void ShowAlertDialog(string message)
    {
        alertMessage.text = message;
        alertObject.SetActive(true);
    }
    public void HideAlertDialog()
    {
        alertObject.SetActive(false);
    }
    public void OnClickOkButton()
    {
        alertObject.SetActive(false);
        SceneManager.LoadScene(sceneLoaded);
    }

    public void ShowLoading()
    {
        loadingRequestCount++;
        loadingObject.SetActive(true);
    }
    public void HideLoading()
    {
        loadingRequestCount--;
        print(loadingRequestCount);
        if(loadingRequestCount == 0)
        {
            loadingObject.SetActive(false);
        }
    }

    public void ShowConfirmDialog(string message, System.Action<bool> callback)
    {
        this.callback = callback;
        confirmMessage.text = message;
        confirmObject.SetActive(true);
    }


    public void OnYesButtonClicked()
    {
        confirmObject.SetActive(false);
        callback?.Invoke(true);
    }

    public void OnNoButtonClicked()
    {
        confirmObject.SetActive(false);
        callback?.Invoke(false);
    }
}
