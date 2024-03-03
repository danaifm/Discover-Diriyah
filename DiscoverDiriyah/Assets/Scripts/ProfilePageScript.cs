using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfilePageScript : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseUser user;
    private FirebaseApp app;
    private CollectionReference fs;
    private DocumentReference userinfo;
    private DocumentSnapshot snapshot;
    private Dictionary<string, object> dictionary;
    [SerializeField] public TMP_Text nameText, emailText;
    [SerializeField] private ConfirmationScript confirmationWindow;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("1. in start");
        initializeFirebase();
        Debug.Log(user.UserId);
        fs = FirebaseFirestore.DefaultInstance.Collection("Account");
        Debug.Log("4. before getuserinfo async");
        userinfo = fs.Document(user.UserId);
        getUserInfo(userinfo);
        Debug.Log("7. in start after getuserinfo async");
    }

    private async void getUserInfo(DocumentReference userinfo)
    {
        Debug.Log("5. in getuserinfo async");
        snapshot = await userinfo.GetSnapshotAsync();
        Debug.Log("6. end of getuserinfo async");
        dictionary = snapshot.ToDictionary();
        Debug.Log("8. printing dictionary");
        Debug.Log(dictionary["Name"].ToString());
        Debug.Log(dictionary["Email"].ToString());
        nameText.text = dictionary["Name"].ToString();
        emailText.text = dictionary["Email"].ToString();
    }

    private void initializeFirebase()
    {
        Debug.Log("2. in initialize firebase");
        app = FirebaseApp.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        Debug.Log("3. end of initialize firebase");
    }

    public void ChangeSceneToEditProfile()
    {
        StartCoroutine(LoadSceneEditProfile());
    }

    private IEnumerator LoadSceneEditProfile()
    {
        Debug.Log("IENUMERATOR changing scene to edit profile");
        var loadscene = SceneManager.LoadSceneAsync("EditProfile");
        while (!loadscene.isDone)
        {
            Debug.Log("loading the scene...");
            yield return null;
        }
        Debug.Log("after loading scene");

    }

    private void DeleteAccount()
    {
        string userID = user.UserId;

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("Account").Document(userID);

        docRef.DeleteAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Account document deleted successfully!" + userID);
                user.DeleteAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("deleted user from auth");
                        StartCoroutine(LoadSceneEditProfile());
                    }
                    else
                    {
                        Debug.Log("error deleting user from auth");
                    }
                });
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to delete account document: " + task.Exception);
            }
        });
    }

    private void SignOut()
    {
        confirmationWindow.gameObject.SetActive(false);
        //TODO: ADD SIGNOUT CODE
        Debug.Log("yes clicked");
    }

    private void NoClicked()
    {
        confirmationWindow.gameObject.SetActive(false);
        Debug.Log("no clicked");
    }

    private void openConfirmationWindow(string message, int x)
        //x = 1 sign out
        //x = 2 delete account
    {
        confirmationWindow.gameObject.SetActive(true);
        if (x == 1)
        {
            confirmationWindow.yesButton.onClick.AddListener(SignOut);
            confirmationWindow.noButton.onClick.AddListener(NoClicked);
        }
        else if(x == 2)
        {
            confirmationWindow.yesButton.onClick.AddListener(DeleteAccount);
            confirmationWindow.noButton.onClick.AddListener(NoClicked);
        }
        confirmationWindow.messageText.text = message;
    }

    public void signOutClicked()
    {
        openConfirmationWindow("Are you sure you want to sign out?", 1);
    }

    public void deleteAccountClicked()
    {
        openConfirmationWindow("Are you sure you want to delete your account? You can't undo this action.", 2);
    }
}
