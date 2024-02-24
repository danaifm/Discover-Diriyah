using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
// using Firebase.Database;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System;

public class SigninFirebase1 : MonoBehaviour
{
    //firebase variables
    [Header("Firebase Login")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    //registration variables
    [Space]
   
    public TMP_InputField emailFieldLogin;
   
    public TMP_InputField passwordFieldLogin;

    public TMP_Text errormessagLogin;

    public TMP_Text emailEmptyLogin;
    public TMP_Text passwordEmptyLogin;
    string  emailError, passwordError;


    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out: " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in: " + user.UserId);
            }
        }
    }

    private void Awake()
    {
        auth = FindObjectOfType<SignUpFirebase>().GetComponent<SignUpFirebase>().auth;
        user = FindObjectOfType<SignUpFirebase>().GetComponent<SignUpFirebase>().user;
    }

    public void Login()
    {
        passwordEmptyLogin.text = "";
        emailEmptyLogin.text = "";

        errormessagLogin.text = "";
        Debug.Log(":emailFieldLogin.text " + emailFieldLogin.text);
        Debug.Log("passwordFieldLogin.text " + passwordFieldLogin.text);
     
            Debug.Log("Try login with error ");
            errormessagLogin.text = "Wrong Email or password";
        //StartCoroutine(LoginAsync(emailFieldLogin.text, passwordFieldLogin.text));
        LoginAsync(emailFieldLogin.text, passwordFieldLogin.text);


    }

    public void Logout()
    {
        auth.SignOut();
    }
    private void LoginAsync(string email, string password)
    {
        
        email = email.ToLower();
        int x = 0;
        if (email == "" || email == " ")
        {
            Debug.LogError("email is empty");
            emailEmptyLogin.text = "Email is missing";
            x = 1;

        }
        if (password == "" || password == " ")
        {


            Debug.LogError("password is empty");
            passwordEmptyLogin.text = "Password is missing";
            x = 1;

        }
        if (x < 1) {
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });
        }
    }
    private void CheckAdminStatus(string userId)
    {
        try
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef = db.Collection("users").Document(userId);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
                    Dictionary<string, object> userData = snapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in userData)
                    {
                        Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
                    }
                    string admin = snapshot.GetValue<string>("admin");
                    if (admin == "0")
                    {
                        SceneManager.LoadScene("user_home_page");
                    }
                    else
                    {
                        SceneManager.LoadScene("admin_home_page");
                    }

                }
                else
                {
                    Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
                }
            });

        }
        catch (Exception e)
        {
            Debug.LogError($"An error occurred: {e.Message}");
        }
    }
}
