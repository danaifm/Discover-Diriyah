using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class EditAccountcript : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseUser user;
    private FirebaseApp app;
    private CollectionReference fs;
    private DocumentReference userinfo;
    private DocumentSnapshot snapshot;
    private Dictionary<string, object> dictionary;
    [Header("Account fields")]
    public TMP_InputField nameField;
    public TMP_InputField emailField;
    public TMP_InputField currentPasswordField;
    public TMP_InputField newPasswordField;
    public TMP_Text nameCounter, nameError, emailError, emailCounter, currentPassError, currentPassCounter, newPassError, newPassCounter;
    private bool nameValid, emailValid, currentPassValid = true, newPassValid = true;
    private bool isNewPassVisible = false;
    private bool isCurrPassVisible = false;
    public Image newPasswordIcon, currPasswordIcon;
    public Sprite showPass, hidePass;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("1. in start");
        initializeFirebase();
        fs = FirebaseFirestore.DefaultInstance.Collection("Account");
        nameField.characterLimit = emailField.characterLimit = currentPasswordField.characterLimit = newPasswordField.characterLimit = 50;
        Debug.Log("4. before getuserinfo async");
        userinfo = fs.Document(user.UserId);
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
        Debug.Log(dictionary["Name"].ToString());
        Debug.Log(dictionary["Email"].ToString());
        nameField.text = dictionary["Name"].ToString();
        emailField.text = dictionary["Email"].ToString();
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
        nameCounter.text = nameField.text.Trim().Length + "/50";
        nameError.color = Color.red;
        nameError.fontSize = 3;
        if (nameField.text.Trim() == "")
        {
            nameError.text = "This field cannot be empty.";
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

    public async void validateEmail()
    {
        emailCounter.text = emailField.text.Trim().Length + "/50";
        string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        Regex re = new(strRegex);
        emailError.color = Color.red;
        emailError.fontSize = 3;
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
        await uniqueEmailAsync(emailField.text.Trim().ToLower());
    }

    private async Task uniqueEmailAsync(string email)
    {
        Query query = fs.WhereEqualTo("Email", email);
        var qSnapshot = await query.GetSnapshotAsync();
        if (qSnapshot.Count != 0)
        {
            if (qSnapshot[0].Id != user.UserId) //only if not the user's current email
            {
                emailError.text = "Email is already in use.";
                emailValid = false;
                emailField.image.color = Color.red;
                return;
            }
        }
    }

    private void validateNewPassword()
    {
        //newPassError.color = Color.red;
        //newPassError.fontSize = 3;
        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        //var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
        if (newPasswordField.text.Length < 8)
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

    private IEnumerator validateOldPassword()
    {
        //currentPassError.color = Color.red;
        //currentPassError.fontSize = 3;
        Debug.Log("in ienumerator validateoldpassword");
        Credential credential = EmailAuthProvider.GetCredential(dictionary["email"].ToString(), currentPasswordField.text);
        Task reauthenticate = user.ReauthenticateAsync(credential);
        yield return new WaitUntil(() => reauthenticate.IsCompleted);
        if (reauthenticate.Exception != null)
        {
            Debug.Log("exception with VALIDATE OLD PASSWORD: " + reauthenticate.Exception);
            currentPassError.text = "Incorrect password.";
            currentPassValid = false;
            currentPasswordField.image.color = Color.red;
        }
        else
        {
            Debug.Log("current password correct");
            currentPassError.text = "";
            currentPassValid = true;
            currentPasswordField.image.color = Color.gray;
        }
    }

    private void onSubmitValidatePasswords()
    {
        newPassError.color = currentPassError.color = Color.red;
        newPassError.fontSize = currentPassError.fontSize = 3;
        if(newPasswordField.text != "")
        {
            validateNewPassword();
            if (currentPasswordField.text == "")
            {
                currentPassError.text = "Please enter your current password.";
                currentPassValid = false;
                currentPasswordField.image.color = Color.red;
            }
            else
            {
                StartCoroutine(validateOldPassword());
            }
            return;
        }
        else if (currentPasswordField.text != "" && newPasswordField.text == "")
        {
            newPassError.text = "Please enter a new password";
            newPassValid = false;
            newPasswordField.image.color = Color.red;
            return;
        }
        else
        {
            StartCoroutine(validateOldPassword());
        }
    }

    public void updateAccountInfo()
    {
        StartCoroutine(updateAccountInfoAsync());
    }

    private IEnumerator updateAccountInfoAsync()
    {
        if(newPasswordField.text != "" || currentPasswordField.text != "") //want to change password => validate passwords on submit so i dont get blocked by firebase
        {
            Debug.Log("validating entered passwords");
            onSubmitValidatePasswords();
        }
        if(nameValid && emailValid && newPassValid && currentPassValid)
        {
            Task updateemail = user.UpdateEmailAsync(emailField.text.Trim().ToLower());
            yield return new WaitUntil(() => updateemail.IsCompleted);
            if(updateemail.Exception == null)
            {
                Debug.Log("SUCCESSFULLY updated email in authentication");
            }
            else
            {
                Debug.Log("updating email in authentication FAILED with exception " + updateemail.Exception);
            }

            Dictionary<string, object> newuserinfo = new Dictionary<string, object> { 
                {"Name", nameField.text.Trim()},
                {"Email", emailField.text.Trim().ToLower()},
                {"Admin", "0"}
            };
            if(newPasswordField.text != "" && currentPasswordField.text != "") //will change password
            {
                
                Task updatepass = user.UpdatePasswordAsync(newPasswordField.text);
                yield return new WaitUntil(() => updatepass.IsCompleted);
                if(updatepass.Exception == null)
                {
                    Debug.Log("SUCCESSFULLY updated password in authentication");
                }
                else
                {
                    Debug.Log("updating password in authentication FAILED with exception " + updatepass.Exception);
                }

            }
            Task updatetask = userinfo.UpdateAsync(newuserinfo);
            yield return new WaitUntil(() => updatetask.IsCompleted);
            if(updatetask.Exception == null)
            {
                Debug.Log("SUCCESSFULLY updated account information in firestore");
            }
            else
            {
                Debug.Log("update account FAILED with exception " + updatetask.Exception);
            }
        }
    }

    public void newPasswordCounter()
    {
        newPassCounter.text = newPassCounter.text.Length + "/50";
    }

    public void currPassCounter()
    {
        currentPassCounter.text = currentPassCounter.text.Length + "/50";
    }

    public void toggleNewPassword()
    {
        if (isNewPassVisible)
        {
            // If password is currently visible, hide it
            newPasswordField.contentType = TMP_InputField.ContentType.Password;
            newPasswordIcon.sprite = showPass;
        }
        else
        {
            // If password is currently hidden, show it
            newPasswordField.contentType = TMP_InputField.ContentType.Standard;
            newPasswordIcon.sprite = hidePass;
        }

        // Toggle the visibility flag
        isNewPassVisible = !isNewPassVisible;
        newPasswordField.ForceLabelUpdate();

    }

    public void toggleCurrPassword()
    {
        if (isCurrPassVisible)
        {
            // If password is currently visible, hide it
            currentPasswordField.contentType = TMP_InputField.ContentType.Password;
            currPasswordIcon.sprite = showPass;
        }
        else
        {
            // If password is currently hidden, show it
            currentPasswordField.contentType = TMP_InputField.ContentType.Standard;
            currPasswordIcon.sprite = hidePass;
        }

        // Toggle the visibility flag
        isCurrPassVisible = !isCurrPassVisible;
        currentPasswordField.ForceLabelUpdate();
    }
}
