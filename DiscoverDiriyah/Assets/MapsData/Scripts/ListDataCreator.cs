using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using UnityEngine;
using UnityEngine.UI;

public class ListDataCreator : MonoBehaviour {

    public GameObject prefab;
    public GameObject canvas;
    private Location currentLocation;
    public static ListDataCreator instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RePopulate()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

       // Populate(LocationDataManager.locationData);
    }

    public void Populate( )
    {
        GameObject newObj; // Create GameObject instance
        currentLocation = LocationProviderFactory.Instance.DeviceLocationProvider.CurrentLocation;

        //try
        //{
        //    for (int i = 0; i < locationData.ObjectLocations.Count; i++)
        //    {
        //        string[] location = locationData.ObjectLocations[i].Location.Split(',');

        //        if (DistanceCalculator.IsPointInTheRange(currentLocation.LatitudeLongitude.x,
        //            currentLocation.LatitudeLongitude.y, double.Parse(location[0]), double.Parse(location[1]), LocationDataManager.Radius))
        //        {
        //            newObj = Instantiate(prefab, transform);
        //            newObj.GetComponent<ListItem>().Init(locationData.ObjectLocations[i].ThumbnailURL,
        //                                                  locationData.ObjectLocations[i].Description,
        //                                                  locationData.ObjectLocations[i].ObjectID,
        //                                                  double.Parse(location[0]),
        //                                                  double.Parse(location[1]));
        //        }



        //    }
        //}
        //catch (Exception e)
        //{
        //    Debug.Log(e);
        //}



        float width = canvas.GetComponent<RectTransform>().rect.width;


        Vector2 newSize = new Vector2(width,300);
        GetComponent<GridLayoutGroup>().cellSize = newSize;

    }



}
