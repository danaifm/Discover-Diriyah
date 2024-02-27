using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImap : MonoBehaviour
{

    [SerializeField] Button map;
    // Start is called before the first frame update
    void Start()
    {
       map.onClick.AddListener(MapPage);
    }

    private void MapPage()
    {
        ScenesManager.Instance.LoadMapPage();
    }
    // Start is called before the first frame update
 

}
