using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;

public class toggleFavorite : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseUser user;
    private CollectionReference fs;

    // Start is called before the first frame update
    void Start()
    {
        initializeFirebase();
        fs = FirebaseFirestore.DefaultInstance.Collection("Account").Document(user.UserId).Collection("Favorites");
    }

    void initializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
    }

    public bool initializeFavoriteRestaurant(string restaurantID)
    {
        return isFavoriteRestaurantAsync(restaurantID).Result;
    }

    private async Task<bool> isFavoriteRestaurantAsync(string restaurantID)
    {
        QuerySnapshot query = await fs.WhereEqualTo("ID", restaurantID).GetSnapshotAsync();
        return query.Count != 0; //true -> is favorite (filled heart)
    }

}
