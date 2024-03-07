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
    public List<string> Picture;
    public string Name;
    public bool userFavorite;
}

[Serializable]
public class EventRoot
{
    public string Description;
    public string EndDate;
    public string Location;
    public string Name;
    public string Audience;
    public List<string> Picture;
    public string StartDate;
    public string Price;
    public string WorkingHours;
    public bool userFavorite;
}
[Serializable]
public class AccommodationRoot
{
    public string Description;
    public string Location;
    public float StarRating;
    public string Name;
    public List<string> Picture;
    public bool userFavorite;
}
[Serializable]
public class RestaurantsRoot
{
    public string CuisineType;
    public string Location;
    public List<string> Picture;
    public string Name;
    public bool userFavorite;
    public string ID;
}






