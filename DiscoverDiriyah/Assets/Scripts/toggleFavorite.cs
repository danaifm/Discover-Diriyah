using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;

public class toggleFavorite : MonoBehaviour
{
    private CollectionReference fs;


    //public toggleFavorite()
    //{
    //    fs = FirebaseFirestore.DefaultInstance.Collection("Account").Document(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Collection("Favorites");
    //}

    private void Awake()
    {
        fs = FirebaseFirestore.DefaultInstance.Collection("Account")
            .Document(FirebaseAuth.DefaultInstance.CurrentUser.UserId)
            .Collection("Favorites");
    }

    public async Task<bool> isFavorite(string ID)
    {
        Query query = fs.WhereEqualTo("ID", ID);
        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
        return querySnapshot.Count != 0;
    }

}
