using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class ResetPassword : MonoBehaviour

{

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;


    public void SendPasswordResetEmail()
    {
        string emailAddress = "adoli.606@gmail.com";
       
            auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Password reset email sent successfully.");
            });
        
    }
}
