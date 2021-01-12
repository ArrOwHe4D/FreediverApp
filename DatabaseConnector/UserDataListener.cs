using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreediverApp.DatabaseConnector
{
    public class UserDataListener : Java.Lang.Object, IValueEventListener
    {
        List<User> userList = new List<User>();

        public event EventHandler<UserDataEventArgs> UserDataRetrieved;

        public class UserDataEventArgs : EventArgs 
        {
            internal List<User> Users { get; set; }
        }

        public void GetAllUsers() 
        {
            DatabaseReference userRef = DatabaseConnector.GetDatabase().GetReference("users");
            userRef.AddValueEventListener(this);
        }

        public void Query(string tablename, string field, string value) 
        {
            DatabaseReference userRef = DatabaseConnector.GetDatabase().GetReference(tablename);
            userRef.OrderByChild(field).EqualTo(value).AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null) 
            {
                var records = snapshot.Children.ToEnumerable<DataSnapshot>();
                userList.Clear();

                foreach (DataSnapshot dataRecord in records)
                {
                    User user = new User();
                    user.id = dataRecord.Key;
                    user.username = dataRecord.Child("username").Value.ToString();
                    user.password = dataRecord.Child("password").Value.ToString();
                    user.email = dataRecord.Child("email").Value.ToString();
                    user.firstname = dataRecord.Child("firstname").Value.ToString();
                    user.lastname = dataRecord.Child("lastname").Value.ToString();
                    user.dateOfBirth = dataRecord.Child("birthday").Value.ToString();
                    user.weight = dataRecord.Child("weight").Value.ToString();
                    user.height = dataRecord.Child("height").Value.ToString();
                    userList.Add(user);
                }
                UserDataRetrieved.Invoke(this, new UserDataEventArgs{ Users = userList });
            }
        }
    }
}