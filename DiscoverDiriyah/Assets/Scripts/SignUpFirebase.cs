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

    //------------ Login Part ---------------------
    [Space]
    [Header("Login fields")]
    public TMP_InputField emailFieldLogin;

    public TMP_InputField passwordFieldLogin;
    public TMP_Text emailLoginLength;
    public TMP_Text passwordLoginLength;
    public TMP_Text errormessagLogin;

    public TMP_Text emailEmptyLogin;
    public TMP_Text passwordEmptyLogin;

    public Image passwordStateIcon;
    public Sprite showPassword;
    public Sprite hidePassword;
    private bool isPasswordVisible = false;
    //--------------------------------------------
    private void Start()
    {
        Debug.Log("STARTING APP");
        initializeFirebase();
        nameField.characterLimit = 15;
        emailFieldLogin.characterLimit = 50;
        passwordFieldLogin.characterLimit = 50;
        db = FirebaseFirestore.DefaultInstance.Collection("Account");
       // no need to open/ close connection
    }

    private void Update()
    {
        ValidateLoginLength();
    }
    public void Logout()
    {
        auth.SignOut();
        if (SceneManager.sceneCount != 0)
        {
            SceneManager.LoadScene(0);
        }
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
        //else if (!(uniqueEmail(emailFieldLogin.text.Trim().ToLower()) == 0))
        //{
        //    emailError.text = "Email is already in use.";
        //    emailValid = false;
        //    emailFieldLogin.image.color = Color.red;
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


    //----------------------- Login Methods
    public void Login()
    {
        string email = emailFieldLogin.text;
        string password = passwordFieldLogin.text;
        StartCoroutine(LoginAsync(email, password));
    }
    private IEnumerator LoginAsync(string email, string password)
    {
        ValidateLoginLength();
        int x = 0;
        if (string.IsNullOrEmpty(email) || email.Trim() == "")
        {
            Debug.LogError("email is empty");
            emailEmptyLogin.text = "Field cannot be empty";
            x = 1;
        }
        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("password is empty");
            passwordEmptyLogin.text = "Field cannot be empty";
            x = 1;
        }

        if (x < 1)
        {
            var signInTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => signInTask.IsCompleted);

            if (signInTask.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                passwordEmptyLogin.text = "Sign in was canceled.";
                yield break;
            }
            if (signInTask.IsFaulted)
            {
                passwordEmptyLogin.text = "Email - Password is wrong";
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error");
                yield break;
            }

            FirebaseUser user = signInTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            CheckAdminStatus(user.UserId);
            yield break;
        }
    }

    private void ValidateLoginLength()
    {
        //print("validate login length");
        emailLoginLength.text = emailFieldLogin.text.Length + "/" + emailFieldLogin.characterLimit;
        passwordLoginLength.text = passwordFieldLogin.text.Length + "/" + passwordFieldLogin.characterLimit;
    }

    private void CheckAdminStatus(string userId)
    {
        try
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef = db.Collection("Account").Document(userId);
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
                    string admin = snapshot.GetValue<string>("Admin");
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

    public void ShowPasswordToggle()
    {
        if (isPasswordVisible)
        {
            // If password is currently visible, hide it
            passwordFieldLogin.contentType = TMP_InputField.ContentType.Password;
            passwordStateIcon.sprite = showPassword;
        }
        else
        {
            // If password is currently hidden, show it
            passwordFieldLogin.contentType = TMP_InputField.ContentType.Standard;
            passwordStateIcon.sprite = hidePassword;
        }

        // Toggle the visibility flag
        isPasswordVisible = !isPasswordVisible;
        passwordFieldLogin.ForceLabelUpdate();
    }
    //--------------- End Login Methods
}
