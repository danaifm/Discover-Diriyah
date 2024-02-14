using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
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
        Debug.Log("asswordField.text " + passwordField.text);
        StartCoroutine(LoginAsync( emailField.text,passwordField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
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
            AuthResult authResult = loginTask.Result;
            user = authResult.User;

            Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);

            Debug.LogFormat("{0} You Are Successfully Logged In", user);
            // References.userName = user.DisplayName;
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
    }
    }
}
