using Firebase.Database;
using System;
using System.Linq;

namespace FreediverApp.DatabaseConnector
{
    class LoginInfoListener : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;
        User userdata;

        public LoginInfoListener(EventHandler OnChange) => this.OnChange = OnChange;

        public class LoginInfoEventArgs : EventArgs 
        {
            public LoginInfoEventArgs(User userdata) 
            {
                this.userdata = userdata;
            }

            public User userdata { get; set; }
        }

        public void Create() 
        {
            DatabaseReference loginInfoRef = DatabaseConnector.GetDatabase().GetReference("users");
            loginInfoRef.AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            //var data = snapshot.Children.ToEnumerable<DataSnapshot>();
            //User user = new User();

            //foreach (DataSnapshot record in snapshot.Children.ToEnumerable()) 
            //{
            //    //var user = record.GetValue(Java.Lang.Class.FromType(typeof(User))) as User;
            //}


            //if (OnChange != null && snapshot.Value != null && snapshot.HasChild(username))
            //{
            //   // OnChange.Invoke(this, new LoginInfoEventArgs(true, false));
            //}
            //else if (OnChange != null && snapshot.Value != null && snapshot.HasChild(email)) 
            //{
            //    //OnChange.Invoke(this, new LoginInfoEventArgs());
            //}
        }
    }
}