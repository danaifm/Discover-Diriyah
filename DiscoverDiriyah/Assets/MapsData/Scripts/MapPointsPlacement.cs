using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

public class MapPointsPlacement : MonoBehaviour {

    [SerializeField]
    [Geocode]
    string[] _TrackPartsLatitudeLongitude;
   

    public Vector2d[] _locations;
    [SerializeField]
    AbstractMap _map;
    public int _spawnScale = 1;
    public static List<GameObject> _spawnedObjects;
    Vector2d[] _coordinates;
    bool instatiatedmap = false;
    public GameObject CheckpointIndicator;


    public static MapPointsPlacement instance;
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
    public void PlacePoints(string[] locationsData, List<LocationsData> locationsData1)
    {
        _locations = new Vector2d[locationsData.Length];
        _spawnedObjects = new List<GameObject>();

        int count = 0;
        Debug.Log("locationsData length... "+locationsData.Length);

        for (int i = 0; i < locationsData.Length; i++)
        {
            Debug.Log("PlacePoints lat/long " + locationsData1[i].Latitude+"/"+ locationsData1[i].Longitude);
            _locations[count] = Conversions.StringToLatLon(locationsData[i]);

            var mapPoint = Instantiate(CheckpointIndicator);
            mapPoint.transform.position = _map.GeoToWorldPosition(_locations[count], false);
            mapPoint.GetComponentInChildren<MapItem>().Init(locationsData1[i]);
            Debug.LogWarning(_map.GeoToWorldPosition(_locations[count], true));

            _spawnedObjects.Add(mapPoint);
            count++;

        }
        instatiatedmap = true;

    }


    private void LateUpdate()
    {
        if (instatiatedmap)
        {
            int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }
        }
    }


}
