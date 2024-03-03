using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminFunctionalityManager : MonoBehaviour
{
    public GameObject[] AdminObjects;
    public GameObject[] UserObjects;
    public static bool Admin = false;
    void Start()
    {
        Admin = false;
        AdminObjectsManager(Admin);
    }
    private void AdminObjectsManager(bool value)
    {
        foreach (GameObject Obj in AdminObjects)
        {
            Obj.SetActive(value);
        }
        foreach (GameObject Obj in UserObjects)
        {
            Obj.SetActive(!value);
        }
    }
}
