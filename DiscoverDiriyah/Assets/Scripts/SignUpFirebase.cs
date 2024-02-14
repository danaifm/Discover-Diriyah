using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using System.Text.RegularExpressions;

public class SignUpFirebase : MonoBehaviour
{
    //firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    //registration variables
    [Space]
    [Header("Registration fields")]
    public TMP_InputField nameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_Text nameError;
    private bool nameValid;

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
        StartCoroutine(RegisterAsync(nameField.text, emailField.text, passwordField.text));
    }

  

    public void validateName()
    {
        Regex r = new Regex("^[a-zA-Z0-9\\s]*$");
        if (nameField.text.Trim() == "")
        {
            nameError.text = "Name cannot be empty.";
            nameValid = false;
            nameField.image.color = Color.red;
            return;
        }
        else if (nameField.text.Trim().Length > 15)
        {
            nameError.text = "Name cannot be longer than 15 characters.";
            nameValid = false;
            nameField.image.color = Color.red;
            return;
        }
        else if (!r.IsMatch(nameField.text.Trim()))
        {
            nameError.text = "Name must only contain alphabet, numbers, and spaces.";
            nameValid = false;
            nameField.image.color = Color.red;
            return;
        }
        nameError.text = "";
        nameValid = true;
        nameField.image.color = Color.gray;
    }

    private IEnumerator RegisterAsync(string name, string email, string password)
    {
        validateName();
        if (!nameValid){
            Debug.LogError("Registration FAILED due to invalid inputs");
        }
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
