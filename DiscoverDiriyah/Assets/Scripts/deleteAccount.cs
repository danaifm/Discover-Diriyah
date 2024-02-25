using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.Networking;
using System;
using Firebase.Firestore;
using Vuforia;


public class deleteAccount : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseUser user;
    public string userID;

   

        public void DeleteAccount()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        userID = user.UserId;
        //userID = "ATpoagFEmbZgeK6HP6jwwb6VT7B2";

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("users").Document(userID);

        docRef.DeleteAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Account document deleted successfully!" + userID);
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to delete account document: " + task.Exception);
            }
        });






    }
}
