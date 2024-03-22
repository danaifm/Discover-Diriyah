using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonClasses : MonoBehaviour
{
}

[Serializable]
public class AttractionsRoot
{
    public string Description;
    public string Location;
    public double Latitude;
    public double Longitude;
    public List<string> Picture;
    public string Name;
    public bool userFavorite;
    public string ID;
}

[Serializable]
public class EventRoot
{
    public string Description;
    public string EndDate;
    public string Location;
    public double Latitude;
    public double Longitude;
    public string Name;
    public string Audience;
    public List<string> Picture;
    public string StartDate;
    public string Price;
    public string WorkingHours;
    public bool userFavorite;
    public string ID;
}
[Serializable]
public class AccommodationRoot
{
    public string Description;
    public string Location;
    public double Latitude;
    public double Longitude;
    public float StarRating;
    public string Name;
    public List<string> Picture;
    public bool userFavorite;
    public string ID;
}
[Serializable]
public class RestaurantsRoot
{
    public string CuisineType;
    public string Location;
    public double Latitude;
    public double Longitude;
    public List<string> Picture;
    public string Name;
    public bool userFavorite;
    public string ID;
}
[Serializable]
public class LocationsData
{
    public string Title;
    public string Catagory;
    public string Picture;
    public string Location;
    public double Latitude;
    public double Longitude;
}





