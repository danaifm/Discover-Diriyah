using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionInstance : MonoBehaviour
{
    public static ViewMissions instance;
    public static GameObject self;
    private void Awake() {
        self = gameObject;
        DontDestroyOnLoad(gameObject);
    }
    public void SetInstance(ViewMissions mission)
    {
        instance = mission;
        DontDestroyOnLoad(instance);
    }
}
