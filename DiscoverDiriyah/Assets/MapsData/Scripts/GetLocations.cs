using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
using UnityEngine;

public class GetLocations : MonoBehaviour
{
    public List<LocationsData> locationsList = new List<LocationsData>();
    //LocationsList
    FirebaseFirestore db;
    void Start()
    {
        GetAccommodationsData();
    }
    public void GetAccommodationsData()
    {
        db = FirebaseFirestore.DefaultInstance;
        db.Collection("Accommodation").GetSnapshotAsync().ContinueWithOnMainThread(async task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data: " + task.Exception);
                return;
            }
            foreach (var document in task.Result.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();

                //if (data.ContainsKey("Pictures"))
                //{
                //    List<object> yourArray = (List<object>)data["Pictures"];

                //}
                string json = JsonConvert.SerializeObject(data);
                AccommodationRoot AccommodationsRoot = JsonUtility.FromJson<AccommodationRoot>(json);
                if (AccommodationsRoot.Latitude>0 && AccommodationsRoot.Longitude>0)
                {
                    LocationsData locationsData = new LocationsData();
                    locationsData.Title = AccommodationsRoot.Name;
                    locationsData.Catagory = "Accommodation";
                    if (AccommodationsRoot.Picture.Count > 0)
                    {
                        locationsData.Picture = AccommodationsRoot.Picture[0];
                    }
                    else
                    {
                        locationsData.Picture = "Not Found";
                    }
                    locationsData.Location = AccommodationsRoot.Location;
                    locationsData.Latitude = AccommodationsRoot.Latitude;
                    locationsData.Longitude = AccommodationsRoot.Longitude;
                    locationsList.Add(locationsData);
                }
                else
                {
                    Debug.Log("No Lat/Long found");
                }
                //AccommodationsData.Add(AccommodationsRoot);
                // Log the JSON string
                Debug.Log("JSON data: " + json);
                //documentsList.Add(data);
                Debug.Log("/////////////////////////////////");
                
            }
            GetAttractionsData();
            //LocationDataManager.instance.PlacePoints(locationsList);
            //DataHandler();
        });
    }
    public void GetAttractionsData()
    {
        db = FirebaseFirestore.DefaultInstance;
        db.Collection("Attraction").GetSnapshotAsync().ContinueWithOnMainThread(async task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data: " + task.Exception);
                return;
            }
            foreach (var document in task.Result.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();

                //if (data.ContainsKey("Pictures"))
                //{
                //    List<object> yourArray = (List<object>)data["Pictures"];

                //}
                string json = JsonConvert.SerializeObject(data);
                AttractionsRoot AttractionsRoot = JsonUtility.FromJson<AttractionsRoot>(json);
                if (AttractionsRoot.Latitude > 0 && AttractionsRoot.Longitude > 0)
                {
                    LocationsData locationsData = new LocationsData();
                    locationsData.Title = AttractionsRoot.Name;
                    locationsData.Catagory = "Attraction";
                    if (AttractionsRoot.Picture.Count > 0)
                    {
                        locationsData.Picture = AttractionsRoot.Picture[0];
                    }
                    else
                    {
                        locationsData.Picture = "Not Found";
                    }
                    locationsData.Location = AttractionsRoot.Location;
                    locationsData.Latitude = AttractionsRoot.Latitude;
                    locationsData.Longitude = AttractionsRoot.Longitude;
                    locationsList.Add(locationsData);
                }
                else
                {
                    Debug.Log("No Lat/Long found");
                }
                //AccommodationsData.Add(AccommodationsRoot);
                // Log the JSON string
                Debug.Log("JSON data: " + json);
                //documentsList.Add(data);
                Debug.Log("/////////////////////////////////");
            }
            GetEventsData();
            //DataHandler();
        });
    }
    public void GetEventsData()
    {
        db = FirebaseFirestore.DefaultInstance;
        db.Collection("Event").GetSnapshotAsync().ContinueWithOnMainThread(async task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data: " + task.Exception);
                return;
            }
            foreach (var document in task.Result.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();
                //if (data.ContainsKey("Pictures"))
                //{
                //    List<object> yourArray = (List<object>)data["Pictures"];

                //}
                string json = JsonConvert.SerializeObject(data);
                EventRoot EventsRoot = JsonUtility.FromJson<EventRoot>(json);
                if (EventsRoot.Latitude > 0 && EventsRoot.Longitude > 0)
                {
                    LocationsData locationsData = new LocationsData();
                    locationsData.Title = EventsRoot.Name;
                    locationsData.Catagory = "Event";
                    if (EventsRoot.Picture.Count > 0)
                    {
                        locationsData.Picture = EventsRoot.Picture[0];
                    }
                    else
                    {
                        locationsData.Picture = "Not Found";
                    }
                    locationsData.Location = EventsRoot.Location;
                    locationsData.Latitude = EventsRoot.Latitude;
                    locationsData.Longitude = EventsRoot.Longitude;
                    locationsList.Add(locationsData);
                }
                else
                {
                    Debug.Log("No Lat/Long found");
                }
                //AccommodationsData.Add(AccommodationsRoot);
                // Log the JSON string
                Debug.Log("JSON data: " + json);
                //documentsList.Add(data);
                Debug.Log("/////////////////////////////////");
            
            }
            GetRestaurantsData();
            //DataHandler();
        });
    }
    public void GetRestaurantsData()
    {
        db = FirebaseFirestore.DefaultInstance;
        db.Collection("Restaurant").GetSnapshotAsync().ContinueWithOnMainThread(async task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data: " + task.Exception);
                return;
            }
            foreach (var document in task.Result.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();
                string json = JsonConvert.SerializeObject(data);
                RestaurantsRoot restaurantsRoot = JsonUtility.FromJson<RestaurantsRoot>(json);
                if (restaurantsRoot.Latitude > 0 && restaurantsRoot.Longitude > 0)
                {
                    LocationsData locationsData = new LocationsData();
                    locationsData.Title = restaurantsRoot.Name;
                    locationsData.Catagory = "Restaurant";
                    if (restaurantsRoot.Picture.Count > 0)
                    {
                        locationsData.Picture = restaurantsRoot.Picture[0];
                    }
                    else
                    {
                        locationsData.Picture = "Not Found";
                    }
                    locationsData.Location = restaurantsRoot.Location;
                    locationsData.Latitude = restaurantsRoot.Latitude;
                    locationsData.Longitude = restaurantsRoot.Longitude;
                    locationsList.Add(locationsData);
                }
                else
                {
                    Debug.Log("No Lat/Long found");
                }
                //AccommodationsData.Add(AccommodationsRoot);
                // Log the JSON string
                Debug.Log("JSON data: " + json);
                //documentsList.Add(data);
                Debug.Log("/////////////////////////////////");
            }
            LocationDataManager.instance.PlacePoints(locationsList);
            //DataHandler();
        });
    }
}
