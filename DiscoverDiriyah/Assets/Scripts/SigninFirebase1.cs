using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Firestore;
using System;

public class SigninFirebase1 : MonoBehaviour
{
    //firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    //registration variables
    [Space]
   
    public TMP_InputField emailField;
   
    public TMP_InputField passwordField;

    public Text errormessag;

    public Text emailEmpty;
    public Text passwordEmpty;
    string  emailError, passwordError;

    void initializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

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
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                initializeFirebase();
            }
            else
            {
                Debug.LogError("could not resolve firebase dependencies: " + dependencyStatus);
            }
        }
            );
    }

    public void Login()
    {
        passwordEmpty.text = "";
        emailEmpty.text = "";

        errormessag.text = "";
        Debug.Log(":emailField.text " + emailField.text);
        Debug.Log("passwordField.text " + passwordField.text);
     
            Debug.Log("Try login with error ");
            errormessag.text = "Wrong Email or password";
            StartCoroutine(LoginAsync(emailField.text, passwordField.text));
        
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        email = email.ToLower();
        int x = 0;
        if (email == "" || email == " ")
        {
            Debug.LogError("email is empty");
            emailEmpty.text = "Email is missing";
            x = 1;

        }
        if (password == "" || password == " ")
        {


            Debug.LogError("password is empty");
            passwordEmpty.text = "Password is missing";
            x = 1;

        }
        if (x < 1) { 
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
                errormessag.text = "Login Failed!";
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "Login Failed! Because ";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    errormessag.text = "Wrong Email or password";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong Password";
                    errormessag.text = "Wrong Email or password";
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email is missing";
                    emailEmpty.text = "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password is missing";
                    passwordEmpty.text = "Password is missing";
                    break;
                default:
                    failedMessage = "Login Failed";
                    errormessag.text = "Wrong Email or password";
                    break;
            }

            Debug.Log(failedMessage);
        }
        else
        {
                errormessag.text = "";
                AuthResult authResult = loginTask.Result;
                if(authResult != null)
    {
                    user = authResult.User;

                    Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);
                    CheckAdminStatus(user.UserId);
                //    bool isAdmin = RetrieveAdminStatus(user.UserId);


                //     //int isAdmin = 0;
                //     if (isAdmin)
                //     {
                //         UnityEngine.SceneManagement.SceneManager.LoadScene("AdminScene");
                //     } else
                //     {
                //         UnityEngine.SceneManagement.SceneManager.LoadScene("UserScene");
                //     }

                    
                   
                //     UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("WelcomeScreen");
                }
            }
    }
    }
private void CheckAdminStatus(string userId)
{
    try
    {
        FirebaseFirestore db = FirebaseFirestore.GetInstance("default");

        // if (db != null)
        // {
        //     CollectionReference usersCollection = db.Collection("users");

        //     if (usersCollection != null)
        //     {
        //         // usersCollection.Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //         // {
        //         //     if (task.IsCompleted)
        //         //     {
        //         //         DocumentSnapshot snapshot = task.Result;

        //         //         if (snapshot.Exists)
        //         //         {
        //         //             object adminValue;
        //         //             if (snapshot.TryGetValue("admin", out adminValue) && adminValue is bool)
        //         //             {
        //         //                 bool isAdmin = (bool)adminValue;

        //         //                 if (isAdmin)
        //         //                 {
        //         //                     Debug.Log("User is an admin.");
        //         //                     UnityEngine.SceneManagement.SceneManager.LoadScene("adminScene");
        //         //                 }
        //         //                 else
        //         //                 {
        //         //                     Debug.Log("User is not an admin.");
        //         //                     UnityEngine.SceneManagement.SceneManager.LoadScene("userScene");
        //         //                 }
        //         //             }
        //         //             else
        //         //             {
        //         //                 Debug.LogError("Failed to retrieve 'admin' field or it is not a boolean value.");
        //         //             }
        //         //         }
        //         //     }
        //         // });
        //     }
        //     else
        //     {
        //         Debug.LogError("usersCollection is null.");
        //     }
        // }
        // else
        // {
        //     Debug.LogError("Firestore db is null.");
        // }
    }
    catch (Exception e)
    {
        Debug.LogError($"An error occurred: {e.Message}");
    }
}
}
