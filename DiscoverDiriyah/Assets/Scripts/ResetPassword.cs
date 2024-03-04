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
    private bool emailRegistered;

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

    public  void StartResetPasswordCoroutine()
    {
        StartCoroutine(SendPasswordResetEmail());
    }
    public async void validateEmail(String email)
    {
        emailResetPasswordLength.text = emailFieldResetPassword.text.Trim().Length + "/50";

        //for unique email

        Query query = fs.WhereEqualTo("Email", email.Trim().ToLower());

        var qSnapshot = await query.GetSnapshotAsync();


        string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        Regex re = new(strRegex);
        if (emailFieldResetPassword.text.Trim() == "")
        {

            emailError.text = "This field cannot be empty.";
            emailSentMessage.text = "";
            emailError.color = Color.red;
            emailError.fontSize = 3;
            emailValid = false;
           // emailRegistered = false;
            emailFieldResetPassword.image.color = Color.red;
            return;
        }
         if (!re.IsMatch(emailFieldResetPassword.text.Trim()))
        {
            emailError.text = "Please enter a valid email.";
            emailSentMessage.text = "";
            emailError.color = Color.red;
            emailError.fontSize = 3;
            emailValid = false;
            //emailRegistered = false;
            emailFieldResetPassword.image.color = Color.red;
            return;
        }

        if (qSnapshot.Count == 0)
            {
                emailError.text = "email not registered";
                emailSentMessage.text = "";
                emailError.color = Color.red;
                emailError.fontSize = 3;
                emailError.fontSize = 3;
                emailValid = false;
                //emailValid = false;
                emailFieldResetPassword.image.color = Color.red;
                return;
            }
        emailError.text = "";
        emailError.color = Color.green;
        emailValid = true;
        //emailRegistered = true;
        //emailRegistered = true; 
        emailFieldResetPassword.image.color = Color.gray;
        //uniqueEmailAsync(emailFieldResetPassword.text.Trim().ToLower());
    }

   // public async void uniqueEmailAsync(string email)
   /// {
   // Query query = fs.WhereEqualTo("Email", email);
  //  var qSnapshot = await query.GetSnapshotAsync();

   // if (qSnapshot.Count == 0)
    //{
     // emailError.text = "Please enter a registered email";
     // emailSentMessage.text = "";
    // emailError.color = Color.red;
   //  emailError.fontSize = 3;
   //  emailError.fontSize = 3;
   // emailRegistered = false;
     //emailValid = false;
    // emailFieldResetPassword.image.color = Color.red;
     //return;
   // }
      // emailError.text = "";
       // emailValid = true;
       // emailRegistered = true;
       //emailFieldResetPassword.image.color = Color.gray;

   // }

    private IEnumerator SendPasswordResetEmail()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        string emailAddress = emailFieldResetPassword.text;
       
        validateEmail(emailAddress);
        emailSentMessage.text = " ";
        Debug.Log("email received" + emailAddress );

       // if (!emailValid)
       // {
          //  Debug.LogError("Registration FAILED due to invalid inputs 1");
            // emailSentMessage.text = ""
         //   yield break;
            

       // }

        if (emailValid )
            {
            var task = auth.SendPasswordResetEmailAsync(emailAddress);

            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                yield break;
            }

            emailSentMessage.text = "Reset password email is sent successfully, Please check your email inbox to continue.";
            Debug.Log("Password reset email sent successfully.");

           // auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task =>
              //  {
                 //   if (task.IsCanceled)
                  //  {
                   //     Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                     
                  //      return;
                  //  }
                   // if (task.IsFaulted)
                   // {
                        //Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                     
                       // return;
                 //   }
                   
               // });
                    }
        else
        {
            Debug.LogError("Registration FAILED due to invalid inputs 1");
        }
    }




        }
   

