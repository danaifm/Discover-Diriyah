
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MapItem : MonoBehaviour {

    public LocationsData _locationsData;
    public MeshRenderer PinMesh;
    public Material[] CatagoryMaterial;

    public void Init(LocationsData locationsData)
    {
        _locationsData = locationsData;
        AssignImage(locationsData.Catagory);
    }
    private void AssignImage(string CatagoryName)
    {
        if (CatagoryName == "Accommodation")
        {
            PinMesh.material = CatagoryMaterial[0];
        }
        else if (CatagoryName == "Attraction")
        {
            PinMesh.material = CatagoryMaterial[1];
        }
        else if (CatagoryName == "Event")
        {
            PinMesh.material = CatagoryMaterial[2];
        }
        else if (CatagoryName == "Restaurant")
        {
            PinMesh.material = CatagoryMaterial[3];
        }
        else
        {
            PinMesh.material = CatagoryMaterial[0];
        }
    }
    //private void TaskOnClick(string location)
    //{
    //    PathDrawer.instance.popup.SetActive(true);
    //    PathDrawer.instance.targetLocation = location;
    //    PathDrawer.instance.target = transform;
    //}
    void OnMouseDown()
    {
        PinDataManager.instance.DisplayPinData(_locationsData);
    }

}
