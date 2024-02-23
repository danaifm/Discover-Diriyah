using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using System.Text.RegularExpressions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    public TMP_Text nameError, emailError, passwordError;
    public TMP_Text nameCounter;
    private bool nameValid, emailValid, passwordValid;
    private CollectionReference db;
    private FirebaseApp app;

    private void Start()
    {
        Debug.Log("STARTING APP");
        initializeFirebase();
        nameField.characterLimit = 15;
        db = FirebaseFirestore.DefaultInstance.Collection("users");
       // no need to open/ close connection
    }

    void initializeFirebase() {
        app = FirebaseApp.DefaultInstance;
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

    //private void Awake()
    //{
    //    Debug.Log("IN AWAKE()");
    //    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    //    {
    //        dependencyStatus = task.Result;
    //        if (dependencyStatus == DependencyStatus.Available)
    //        {
    //            initializeFirebase();
    //        }
    //        else
    //        {
    //            Debug.LogError("could not resolve firebase dependencies: " + dependencyStatus);
    //        }
    //    }
    //        );
    //}



    public void Register()
    {
        StartCoroutine(RegisterAsync(nameField.text.Trim(), emailField.text.Trim().ToLower(), passwordField.text));
    }

    public void validateName()
    {
        Regex r = new Regex("^[a-zA-Z0-9\\s]*$");
        nameCounter.text = nameField.text.Trim().Length + "/15";
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

    public void validateEmail()
    {
        string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        Regex re = new(strRegex);
        if (emailField.text.Trim() == "")
        {
            emailError.text = "Email cannot be empty.";
            emailValid = false;
            emailField.image.color = Color.red;
            return;
        }
        else if (!re.IsMatch(emailField.text.Trim()))
        {
            emailError.text = "Please enter a valid email.";
            emailValid = false;
            emailField.image.color = Color.red;
            return;
        }
        //else if (!(uniqueEmail(emailField.text.Trim().ToLower()) == 0))
        //{
        //    emailError.text = "Email is already in use.";
        //    emailValid = false;
        //    emailField.image.color = Color.red;
        //    return;
        //}
        emailError.text = "";
        emailValid = true;
        emailField.image.color = Color.gray;
        uniqueEmailAsync(emailField.text.Trim().ToLower());
    }

    public async void uniqueEmailAsync(string email)
    {
        Query query = db.WhereEqualTo("email", email);
        var qSnapshot = await query.GetSnapshotAsync();
        if (qSnapshot.Count != 0)
        {
            emailError.text = "Email is already in use.";
            emailValid = false;
            emailField.image.color = Color.red;
            return;
        }
    }



    public void validatePassword()
    {
        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        //var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
        if (passwordField.text.Trim() == "")
        {
            passwordError.text = "Password cannot be empty.";
            passwordValid = false;
            passwordField.image.color = Color.red;
            return;
        }
        else if (passwordField.text.Length < 8)
        {
            passwordError.text = "Password must be at least 8 characters.";
            passwordValid = false;
            passwordField.image.color = Color.red;
            return;
        }
        else if (!hasNumber.IsMatch(passwordField.text) || !hasUpperChar.IsMatch(passwordField.text))
        {
            passwordError.text = "Password must contain at least one digit and one uppercase letter.";
            passwordValid = false;
            passwordField.image.color = Color.red;
            return;
        }
        passwordError.text = "";
        passwordValid = true;
        passwordField.image.color = Color.gray;
    }

    private IEnumerator RegisterAsync(string name, string email, string password)
    {
        Debug.Log(name);
        Debug.Log(email);
        Debug.Log(password);
        validateName();
        validateEmail();
        validatePassword();
        if (!nameValid || !emailValid || !passwordValid)
        {
            Debug.LogError("Registration FAILED due to invalid inputs 1");
        }
        else
        { 
                Task<AuthResult> registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
                yield return new WaitUntil(() => registerTask.IsCompleted);
                if (registerTask.Exception != null)
                {
                    Debug.LogError("exception while registering user: " + registerTask.Exception);
                    FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;
                    Debug.Log("Registration failed due to exception: " + authError);

                }
                else
                {
                    user = registerTask.Result.User;
                if (user != null)
                {
                    UserProfile userProfile = new UserProfile { DisplayName = name };
                    Task profileTask = user.UpdateUserProfileAsync(userProfile);
                    yield return new WaitUntil(() => profileTask.IsCompleted);

                    if (profileTask.Exception != null) //setting username fails
                    {
                        Debug.LogError(message: $"Failed to set username with exception: {profileTask.Exception}");

                    }
                    else //setting username success
                    {
                        Dictionary<string, string> userinfo = new Dictionary<string, string>
                        {
                            {"name", name},
                            {"email", email},
                            {"admin", "0"}
                        };
                        
                        db.Document(user.UserId).SetAsync(userinfo).ContinueWith(task =>
                        {
                            if (task.IsCompletedSuccessfully)
                            {
                                Debug.Log("added user " + user.UserId + " to firestore");
                                SceneManager.LoadScene("ProfilePage");
                            }
                            else
                            {
                                Debug.LogError(message: $"Failed to insert into firestore with exception: {task.Exception}");
                            }
                        }
                        );
                        Debug.Log("registration success!");
                    }

                }
            }
        }
    }

    
}
