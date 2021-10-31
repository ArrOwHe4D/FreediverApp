using Android.App;
using Firebase;
using Firebase.Database;
using Firebase.Firestore;

namespace FreediverApp.DatabaseConnector
{
    /**
     * This Class uses the Nuget Package "Firebase.Database" to establish a connection
     * to our firebase NOSQL database. The static function below "GetDatabase()" handles
     * the connection and can be called anywhere within our app. In order to connect to a different
     * database, the parameters inside the options object need to be changed. But the data structure inside
     * the new databse must be the same as it is inside our testing environment otherwise our database listener
     * will fail to read and write data to the database.
     */
    public static class DatabaseConnector
    {
        public static FirebaseDatabase GetDatabase() 
        {
            var app = FirebaseApp.InitializeApp(Application.Context);

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId("FreeDiverApp")
                    .SetApiKey("AIzaSyCRepKGafGq36s5QLabDOQo46XwdoZcO88")
                    .SetDatabaseUrl("https://freediver-eca91-default-rtdb.europe-west1.firebasedatabase.app/")
                    .SetStorageBucket("freediver-eca91.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
            }

            return FirebaseDatabase.GetInstance(app);
        }

        public static FirebaseFirestore GetFirestoreDatabase()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("freediver-eca91")
                    .SetApplicationId("freediver")
                    .SetApiKey("AIzaSyCRepKGafGq36s5QLabDOQo46XwdoZcO88")
                    .SetDatabaseUrl("https://freediver.firebaseio.com")
                    .SetStorageBucket("freediver-eca91.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
            }

            return FirebaseFirestore.GetInstance(app);
        }
    }
}