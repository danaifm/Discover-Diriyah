using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;

public class SignInFirebase : MonoBehaviour
{
    //firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    //registration variables
    [Space]
    public TMP_InputField nameField;
    public TMP_InputField emailField;
    public TMP_InputField phoneField;
    public TMP_InputField passwordField;
    string nameError, emailError, phoneError, passwordError;

    void initializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if(auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if(!signedIn && user != null)
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
            if(dependencyStatus == DependencyStatus.Available)
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

    public void Register()
    {
        StartCoroutine(RegisterAsync(nameField.text, emailField.text, phoneField.text, passwordField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string phone, string password)
    {
        if (name == "")
            Debug.LogError("name is empty");
        else if (email == "")
            Debug.LogError("email is empty");
        else if (phone == "")
            Debug.LogError("phone is empty");
        else if (password == "")
            Debug.LogError("password is empty");
        else
        {
            Task<AuthResult> registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => registerTask.IsCompleted);
            if(registerTask.Exception != null)
            {
                Debug.LogError("exception while registering user: " + registerTask.Exception);
                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;
                Debug.Log("Registration failed due to exception: " + authError);
                
            }
            else
            {
                user = registerTask.Result.User;
                if(user != null)
                {
                    UserProfile userProfile = new UserProfile { DisplayName = name };
                    Task profileTask = user.UpdateUserProfileAsync(userProfile);
                    yield return new WaitUntil(() => profileTask.IsCompleted);

                    if(profileTask.Exception != null) //setting username fails
                    {
                        Debug.LogError(message: $"Failed to set username with exception: {profileTask.Exception}");

                    }
                    else //setting username success
                    {
                        Debug.LogAssertion("registration success!");
                    }
                     
                }
            }
        }
    }
}
