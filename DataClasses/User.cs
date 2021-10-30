using System;
using System.Collections.Generic;

namespace FreediverApp
{
    /**
     *  This dataclass represents a user with all of the required account data. 
     **/
    public class User
    {
        public string id;
        public string username;
        public string password;
        public string email;
        public string firstname;
        public string lastname;
        public string dateOfBirth;
        public string weight;
        public string height;
        public string gender;
        public string registerdate;

        public List<DiveSession> diveSessions = new List<DiveSession>();

        public User() { }

        public User(List<DiveSession> _diveSessions)
        {
            diveSessions = _diveSessions;
        }

        public User(string id, string username, string email, string password, string firstname, string lastname, string dateOfBirth, string weight, string height, string gender) 
        {
            this.id = id;
            this.username = username;
            this.email = email;
            this.password = password;
            this.firstname = firstname;
            this.lastname = lastname;
            this.dateOfBirth = dateOfBirth;
            this.weight = weight;
            this.height = height;
            this.gender = gender;
            this.registerdate = DateTime.Now.Date.ToString("dd.MM.yyyy");
        }
    }
}