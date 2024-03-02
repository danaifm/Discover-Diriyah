using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using System.Text.RegularExpressions;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetPassword : MonoBehaviour

{
    [Space]
    [Header("Reset Password fields")]
    public TMP_InputField emailFieldResetPassword;
    public TMP_Text emailResetPasswordLength;
    public TMP_Text emailError;
    public TMP_Text emailSentMessage;
    private bool emailValid;

    public FirebaseAuth auth;
    public FirebaseUser user;
    private CollectionReference db, fs;
    private FirebaseApp app;
    private void Start()
    {
        Debug.Log("STARTING APP");
        initializeFirebase();
        emailFieldResetPassword.characterLimit =  50;
        fs = FirebaseFirestore.DefaultInstance.Collection("Account");
        // no need to open/ close connection
    }
    void initializeFirebase()
    {
        app = FirebaseApp.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
     
    }
    public void validateEmail()
    {
        emailResetPasswordLength.text = emailFieldResetPassword.text.Trim().Length + "/50";

        string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        Regex re = new(strRegex);
        if (emailFieldResetPassword.text.Trim() == "")
        {
            emailError.text = "This field cannot be empty.";
            emailError.color = Color.red;
            emailError.fontSize = 3;
            emailValid = false;
            emailFieldResetPassword.image.color = Color.red;
            return;
        }
        else if (!re.IsMatch(emailFieldResetPassword.text.Trim()))
        {
            emailError.text = "Please enter a valid email.";
            emailError.color = Color.red;
            emailError.fontSize = 3;
            emailValid = false;
            emailFieldResetPassword.image.color = Color.red;
            return;
        }
        emailError.text = "";
        emailValid = true;
        emailFieldResetPassword.image.color = Color.gray;
        uniqueEmailAsync(emailFieldResetPassword.text.Trim().ToLower());
    }

    public async void uniqueEmailAsync(string email)
    {
        Query query = fs.WhereEqualTo("Email", email);
        var qSnapshot = await query.GetSnapshotAsync();
        if (qSnapshot.Count == 0)
        {
            emailError.text = "Please enter an already registered email.";
            emailError.color = Color.red;
            emailError.fontSize = 3;
            emailValid = false;
            emailFieldResetPassword.image.color = Color.red;
            return;
        }
    }
    public void SendPasswordResetEmail()
    {
        auth = FirebaseAuth.DefaultInstance;
        validateEmail();
        string emailAddress = emailFieldResetPassword.text;
        Debug.Log("email received" + emailAddress );

        if (!emailValid)
        {
            Debug.LogError("Registration FAILED due to invalid inputs 1");
        }
        else
        {

            auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task =>
            {
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
                if(task.IsCompleted)
                {
                    Debug.Log("email sent");
                    emailSentMessage.text = "Reset Password Email is sent successfully, Please check your email inbox to continue.";
                }
                
            });
        }
    }
}
