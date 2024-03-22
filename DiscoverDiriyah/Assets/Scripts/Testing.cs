using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using UnityEngine;

public class Testing : MonoBehaviour {
    public  Location currentLocation;
    // Use this for initialization
    void Start () {
        Invoke("getlocation",1);
    }
	
	// Update is called once per frame
	void getlocation () {
        currentLocation = LocationProviderFactory.Instance.DeviceLocationProvider.CurrentLocation;
        Debug.Log(currentLocation.LatitudeLongitude);
    }
}
