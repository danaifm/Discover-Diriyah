using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Newtonsoft.Json;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirestoreManager : MonoBehaviour
{
    public delegate void OnFirebaseInitialize();
    public static OnFirebaseInitialize OnFirebaseInitializeDele;

    FirebaseFirestore db;

    void Start()
    {
        // Initialize Firestore
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase is ready to use
                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase Firestore initialized successfully!");
                OnFirebaseInitializeDele?.Invoke();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }
}
