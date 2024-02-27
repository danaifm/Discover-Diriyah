using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIscanQR : MonoBehaviour
{
    [SerializeField] Button scanQR;
    // Start is called before the first frame update
    void Start()
    {
        scanQR.onClick.AddListener(scanQRpage);
    }

    private void scanQRpage()
    {
        ScenesManager.Instance.LoadQRPage();
    }
}
