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
    // Start is called before the first frame update
    //void Start()
    //{
    //    Debug.Log("in togglefav start!!!!!");
    //    user = FirebaseAuth.DefaultInstance.CurrentUser;
    //    fs = FirebaseFirestore.DefaultInstance.Collection("Account").Document(user.UserId).Collection("Favorites");
    //}

    public toggleFavorite()
    {
    }

    public async Task<bool> isFavorite(string ID)
    {
        CollectionReference fs = FirebaseFirestore.DefaultInstance.Collection("Account").Document(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Collection("Favorites");
        Query query = fs.WhereEqualTo("ID", ID);
        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
        return querySnapshot.Count != 0;
    }

}
