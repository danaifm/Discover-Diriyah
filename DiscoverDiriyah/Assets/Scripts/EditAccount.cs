using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class EditAccountcript : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseUser user;
    private FirebaseApp app;
    private CollectionReference db;
    private DocumentReference userinfo;
    private DocumentSnapshot snapshot;
    private Dictionary<string, object> dictionary;
    [Header("Account fields")]
    public TMP_InputField nameField;
    public TMP_InputField emailField;
    public TMP_InputField currentPasswordField;
    public TMP_InputField newPasswordField;
    public TMP_Text nameCounter, nameError, emailError, currentPassError, newPassError;
    private bool nameValid, emailValid, currentPassValid, newPassValid;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("1. in start");
        initializeFirebase();
        nameField.characterLimit = 15;
        //db = FirebaseFirestore.DefaultInstance.Collection("users");
        userinfo = FirebaseFirestore.DefaultInstance.Collection("users").Document(user.UserId);
        Debug.Log("4. before getuserinfo async");
        getUserInfo(userinfo);
        Debug.Log("7. in start after getuserinfo async");
        
    }

    public async void getUserInfo(DocumentReference userinfo)
    {
        Debug.Log("5. in getuserinfo async");
        snapshot = await userinfo.GetSnapshotAsync();
        Debug.Log("6. end of getuserinfo async");
        dictionary = snapshot.ToDictionary();
        Debug.Log("8. printing dictionary");
        Debug.Log(dictionary["name"].ToString());
        Debug.Log(dictionary["email"].ToString());
        nameField.text = dictionary["name"].ToString();
        emailField.text = dictionary["email"].ToString();
    }

    void initializeFirebase()
    {
        Debug.Log("2. in initialize firebase");
        app = FirebaseApp.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        Debug.Log("3. end of initialize firebase");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void validateName()
    {
        Regex r = new Regex("^[a-zA-Z0-9\\s]*$");
        nameCounter.text = nameField.text.Trim().Length + "/15";
        if (nameField.text.Trim() == "")
        {
            nameError.text = "This field cannot be empty.";
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
            emailError.text = "This field cannot be empty.";
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

    public void validateNewPassword()
    {
        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        //var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
        if (newPasswordField.text.Trim() == "" && currentPasswordField.text.Trim() != "") //if enter current pass but not new pass
        {
            newPassError.text = "Password cannot be empty.";
            newPassValid = false;
            newPasswordField.image.color = Color.red;
            return;
        }
        else if (newPasswordField.text.Length < 8)
        {
            newPassError.text = "Password must be at least 8 characters.";
            newPassValid = false;
            newPasswordField.image.color = Color.red;
            return;
        }
        else if (!hasNumber.IsMatch(newPasswordField.text) || !hasUpperChar.IsMatch(newPasswordField.text))
        {
            newPassError.text = "Password must contain at least one digit and one uppercase letter.";
            newPassValid = false;
            newPasswordField.image.color = Color.red;
            return;
        }
        newPassError.text = "";
        newPassValid = true;
        newPasswordField.image.color = Color.gray;
    }

    public void validateCurrentPassword()
    {
        Debug.Log("in void validatecurrentpassword");
        StartCoroutine(validateOldPassword());
    }

    public IEnumerator validateOldPassword()
    {
        Debug.Log("in ienumerator validateoldpassword");
        Credential credential = EmailAuthProvider.GetCredential(dictionary["email"].ToString(), currentPasswordField.text);
        Task reauthenticate = user.ReauthenticateAsync(credential);
        yield return new WaitUntil(() => reauthenticate.IsCompleted);
        if (reauthenticate.Exception != null)
        {
            Debug.Log("exception with validateoldpassword: " + reauthenticate.Exception);
        }
        else
        {
            Debug.Log("no exception");
        }

    }
}
