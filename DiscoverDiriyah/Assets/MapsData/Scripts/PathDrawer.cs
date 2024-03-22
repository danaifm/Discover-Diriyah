using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

public class PathDrawer : MonoBehaviour {

    public GameObject popup;

    [HideInInspector]
    public string targetLocation;

    [HideInInspector]
    public Transform target;

    
    public Transform player;

    public DirectionsFactory directionsFactory;


    public static PathDrawer instance;
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

    private void Start()
    {
        popup.SetActive(false);
    }
    
    public void DrawPath()
    {
        directionsFactory._waypoints[0] = player;
        directionsFactory._waypoints[1] = target;
        directionsFactory.gameObject.SetActive(true);
        //Debug.Log(targetLocation);
    }


}
