using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIprofile : MonoBehaviour
{
    [SerializeField] Button profile;
    // Start is called before the first frame update
    void Start()
    {
        profile.onClick.AddListener(ProfilePage);
    }

    private void ProfilePage()
    {
        ScenesManager.Instance.LoadProfilePage();
    }
}
