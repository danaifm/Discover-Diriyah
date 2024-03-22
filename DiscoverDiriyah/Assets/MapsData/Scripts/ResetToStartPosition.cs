using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System;

public class ResetToStartPosition : MonoBehaviour
{

    [SerializeField]
    AbstractMap _map;
    public Vector2d StartLatLong;
    public float Zoom;
    public LocationProviderFactory lpf;

    void Start()
    {

#if UNITY_EDITOR
        StartLatLong = LocationProviderFactory.Instance.EditorLocationProvider.CurrentLocation.LatitudeLongitude;
#endif
#if !UNITY_EDITOR
         StartLatLong=LocationProviderFactory.Instance.DeviceLocationProvider.CurrentLocation.LatitudeLongitude;
#endif
        _map.Initialize(StartLatLong, (int)Zoom);
        _map.SetCenterLatitudeLongitude(StartLatLong);
        lpf = LocationProviderFactory.Instance;
        _map.UpdateMap(StartLatLong);
    }

    private void startatGpscentre()
    {
        Reset();

    }

    public void Reset()
    {

        Vector2d devicelatlong;


#if UNITY_EDITOR
        devicelatlong = LocationProviderFactory.Instance.EditorLocationProvider.CurrentLocation.LatitudeLongitude;
#endif
#if !UNITY_EDITOR
         devicelatlong=LocationProviderFactory.Instance.DeviceLocationProvider.CurrentLocation.LatitudeLongitude;
#endif


        if (Input.location.isEnabledByUser)
        {
            devicelatlong = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            _map.SetCenterLatitudeLongitude(devicelatlong);
            _map.SetZoom(Zoom);
            _map.UpdateMap();

        }
        else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            devicelatlong = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            _map.SetCenterLatitudeLongitude(devicelatlong);
            _map.SetZoom(Zoom);
            _map.UpdateMap();

        }
    }
}
