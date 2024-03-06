using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;

public class toggleFavorite : MonoBehaviour
{
    public FirebaseUser user;
    private CollectionReference fs;
    private int favRestaurantCount;
    private bool isFav;
    private QuerySnapshot querySnapshot;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("in togglefav start..!!!!!!");
        initializeFirebase();
        fs = FirebaseFirestore.DefaultInstance.Collection("Account").Document(user.UserId).Collection("Favorites");
    }

    void initializeFirebase()
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    public bool isFavorite(string ID)
    {
        Debug.Log("in isfavorite");
        return true;
    }

    public async Task getQuerySnapshot(string ID)
    {
        Query query = fs.WhereEqualTo("ID", ID);
        querySnapshot = await query.GetSnapshotAsync();
    }

}
