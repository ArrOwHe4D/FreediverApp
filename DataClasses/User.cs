﻿using System;
using System.Collections.Generic;

namespace FreediverApp
{
    class User
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
        public string registerdate;

        public List<DiveSession> diveSessions = new List<DiveSession>();
        public DiveSession curDiveSession;
        public Dive curDive;

        public User() 
        {

        }

        public User(List<DiveSession> _diveSessions)
        {
            diveSessions = _diveSessions;
        }

        public User(string id, string username, string email, string password, string firstname, string lastname, string dateOfBirth, string weight, string height) 
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
            this.registerdate = DateTime.Now.Date.ToString("dd.MM.yyyy");
        }
        
        public static User curUser = new User(new List<DiveSession>() { new DiveSession("22.11.2020"), new DiveSession("23.11.2020"), new DiveSession("25.11.2020") });
    }
}