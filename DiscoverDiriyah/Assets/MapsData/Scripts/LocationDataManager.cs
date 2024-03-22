using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocationDataManager : MonoBehaviour
{
    //private const bool V = true;
    public static LocationDataManager instance;

    public string[] m_locationsData;

    public LocationsData[] users;
    public int currentCollectiongIndex;

    private bool isPlaceable;
    public static int LocationCounter;
    public List<LocationsData> UserLocation = new List<LocationsData>();
    //public static MapLocations.Root UserLocation;

    public static double lat_2;
    public static double long_2;
    double distance;



    //public DirectionsFactory directionsFactory;
    private Location currentLocation;
    public static int userLoc;
    public Button Cancebtn;

    float lat_1;
    float long_1;

    public void PlacePoints(List<LocationsData> locationsData)
    {
        Array.Clear(m_locationsData, 0, m_locationsData.Length);
        Array.Clear(users, 0, users.Length);
        UserLocation = locationsData;

        int count = UserLocation.Count;
        m_locationsData = new string[count];
        LocationCounter = count;
        users = new LocationsData[UserLocation.Count];
        //users = new MapLocations.Location[count];

        currentLocation = LocationProviderFactory.Instance.DeviceLocationProvider.CurrentLocation;

        lat_1 = (float)currentLocation.LatitudeLongitude.x;
        long_1 = (float)currentLocation.LatitudeLongitude.y;

        Vector2d fromMeters = Conversions.LatLonToMeters(lat_1, long_1);

        for (int i = 0; i < count; i++)
        {

            m_locationsData[i] = UserLocation[i].Latitude + "," + UserLocation[i].Longitude;
            Debug.Log(UserLocation[i].Latitude + "," + UserLocation[i].Longitude);
            users[i] = UserLocation[i];

            Vector2d toMeters = Conversions.LatLonToMeters(UserLocation[i].Latitude, UserLocation[i].Longitude);
            Debug.Log(lat_2 + long_2);
            distance = (fromMeters - toMeters).magnitude;
            Debug.Log("here is the distance" + distance);


        }
        //canCollect = true;
        Debug.Log("size = " + UserLocation.Count);
        MapPointsPlacement.instance.PlacePoints(m_locationsData, UserLocation);
    }


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        isPlaceable = false;
        LocationProviderFactory.Instance.DeviceLocationProvider.OnLocationUpdated += OnUpdateLocationCalled;
    }

    private void OnUpdateLocationCalled(Location location)
    {
        if (m_locationsData != null)
        {
            if (isPlaceable)
            {
                isPlaceable = false;
            }

        }
    }

    private void OnDestroy()
    {
        LocationProviderFactory.Instance.DeviceLocationProvider.OnLocationUpdated -= OnUpdateLocationCalled;
    }
}





