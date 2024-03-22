using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Directions;
using Mapbox.Unity;
using Mapbox.Json;
using Mapbox.Utils.JsonConverters;
using Mapbox.Unity.Location;
using Mapbox.Utils;

public class RenderMap : MonoBehaviour
{
    [SerializeField]
    AbstractMap _map;// Start is called before the first frame update

  public void RerenderMap() 
 {
        Vector2d devicelatlong;

#if UNITY_EDITOR
        devicelatlong = LocationProviderFactory.Instance.EditorLocationProvider.CurrentLocation.LatitudeLongitude;
#endif
#if !UNITY_EDITOR
         devicelatlong=LocationProviderFactory.Instance.DeviceLocationProvider.CurrentLocation.LatitudeLongitude;
#endif


        _map.UpdateMap(devicelatlong,15);
 }

}
