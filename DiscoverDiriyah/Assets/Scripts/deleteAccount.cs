using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;
using Firebase.Firestore;
using Vuforia;


public class deleteAccount : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseUser user;
    public string userID;
   
    public  void DeleteAccount()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        userID = user.UserId;

       // DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;






    }
}
