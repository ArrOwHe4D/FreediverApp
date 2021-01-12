using Firebase.Database;
using System;
using System.Linq;

namespace FreediverApp.DatabaseConnector
{
    class LoginInfoListener : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;
        string username, email;

        public LoginInfoListener(EventHandler OnChange, string username, string email)
        {
            this.OnChange = OnChange;
            this.username = username;
            this.email = email;
        }

        public class LoginInfoEventArgs : EventArgs 
        {
            public LoginInfoEventArgs(bool usernameFound, bool emailFound) 
            {
                this.usernameFound = usernameFound;
                this.emailFound = emailFound;
            }

            public User userdata { get; set; }

            public bool usernameFound { get; set; }

            public bool emailFound { get; set; }
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
            var data = snapshot.Children.ToEnumerable<DataSnapshot>();
            User user = new User();

            if (OnChange != null && snapshot.Value != null && snapshot.HasChild(username))
            {
                OnChange.Invoke(this, new LoginInfoEventArgs(true, false));
            }
            else if (OnChange != null && snapshot.Value != null && snapshot.HasChild(email)) 
            {
                OnChange.Invoke(this, new LoginInfoEventArgs(false, true));
            }
        }
    }
}