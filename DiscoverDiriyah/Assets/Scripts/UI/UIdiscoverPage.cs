using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class UIdiscoverPage : MonoBehaviour
{
    [SerializeField] Button Home;
    // Start is called before the first frame update
    void Start()
    {
        Home.onClick.AddListener(userDicoverPage);
    }

    private void userDicoverPage()
    {
        ScenesManager.Instance.LoadDicoverPage();
    }

   
}
