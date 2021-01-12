using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Database;

namespace FreediverApp.DatabaseConnector
{
    public static class DatabaseConnector
    {
        public static FirebaseDatabase GetDatabase() 
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseDatabase database;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId("freediverdb")
                    .SetApiKey("AIzaSyBcNaDJVD1HLW_pRqOo_ZRfy0YH72CoLfE")
                    .SetDatabaseUrl("https://freediverdb.firebaseio.com")
                    .SetStorageBucket("freediverdb.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);

                database = FirebaseDatabase.GetInstance(app);
            }
            else 
            {
                database = FirebaseDatabase.GetInstance(app);
            }

            return database;
        }

        public static DatabaseReference GetTable(string tablename) 
        {
            return GetDatabase().GetReference(tablename);
        }
    }
}