﻿using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreediverApp.DatabaseConnector
{
    public class FirebaseDataListener : Java.Lang.Object, IValueEventListener
    {
        List<User> userList = new List<User>();
        List<DiveSession> divesessionList = new List<DiveSession>();
        List<Measurepoint> measurePointList = new List<Measurepoint>();

        public event EventHandler<DataEventArgs> DataRetrieved;

        public class DataEventArgs : EventArgs 
        {
            internal List<User> Users { get; set; }
            internal List<DiveSession> DiveSessions { get; set; }
            internal List<Measurepoint> Measurepoints { get; set; }
        }

        public void QueryFullTable(string tablename) 
        {
            DatabaseReference tableRef = DatabaseConnector.GetDatabase().GetReference(tablename);
            tableRef.AddValueEventListener(this);
        }

        public void QueryParameterized(string tablename, string field, string value) 
        {
            DatabaseReference tableRef = DatabaseConnector.GetDatabase().GetReference(tablename);
            tableRef.OrderByChild(field).EqualTo(value).AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void createEntitiesFromSnapshot(DataSnapshot snapshot) 
        {
            //determine which table was queried 
            //we cannot generalize this completely at the moment -> would be too much since we dont need a full db api for the small amount of tasks
            switch (snapshot.Ref.Key)
            {
                case "users":
                {
                    foreach (DataSnapshot dataRecord in snapshot.Children.ToEnumerable<DataSnapshot>())
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
                    break;
                }
                case "divesessions":
                {
                    foreach (DataSnapshot dataRecord in snapshot.Children.ToEnumerable<DataSnapshot>())
                    {
                        DiveSession divesession = new DiveSession();
                        divesession.date = dataRecord.Child("date").Value.ToString();
                        divesession.duration = dataRecord.Child("duration").Value.ToString();
                        divesession.heartFreqMax = dataRecord.Child("heart_freq_max").Value.ToString();
                        divesession.heartFreqMin = dataRecord.Child("heart_freq_min").Value.ToString();
                        divesession.luminanceMax = dataRecord.Child("luminance_max").Value.ToString();
                        divesession.luminanceMin = dataRecord.Child("luminance_min").Value.ToString();
                        divesession.maxDepth = dataRecord.Child("max_depth").Value.ToString();
                        divesession.oxygenSaturationMax = dataRecord.Child("oxygen_saturation_max").Value.ToString();
                        divesession.oxygenSaturationMin = dataRecord.Child("oxygen_saturation_min").Value.ToString();
                        divesession.refUser = dataRecord.Child("ref_user").Value.ToString();
                        divesession.timestampBegin = dataRecord.Child("timestamp_begin").Value.ToString();
                        divesession.timestampEnd = dataRecord.Child("timestamp_end").Value.ToString();
                        divesession.waterTemperatureMax = dataRecord.Child("water_temp_max").Value.ToString();
                        divesession.waterTemperatureMin = dataRecord.Child("water_temp_min").Value.ToString();
                        divesessionList.Add(divesession);
                    }
                    break;
                }
                case "measurepoints":
                {
                    foreach (DataSnapshot dataRecord in snapshot.Children.ToEnumerable<DataSnapshot>())
                    {
                        Measurepoint measurepoint = new Measurepoint();

                        DiveSession divesession = new DiveSession();
                        divesession.date = dataRecord.Child("date").Value.ToString();
                        divesession.duration = dataRecord.Child("duration").Value.ToString();
                        divesession.heartFreqMax = dataRecord.Child("heart_freq_max").Value.ToString();
                        divesession.heartFreqMin = dataRecord.Child("heart_freq_min").Value.ToString();
                        divesession.luminanceMax = dataRecord.Child("luminance_max").Value.ToString();
                        divesession.luminanceMin = dataRecord.Child("luminance_min").Value.ToString();
                        divesession.maxDepth = dataRecord.Child("max_depth").Value.ToString();
                        divesession.oxygenSaturationMax = dataRecord.Child("oxygen_saturation_max").Value.ToString();
                        divesession.oxygenSaturationMin = dataRecord.Child("oxygen_saturation_min").Value.ToString();
                        divesession.refUser = dataRecord.Child("ref_user").Value.ToString();
                        divesession.timestampBegin = dataRecord.Child("timestamp_begin").Value.ToString();
                        divesession.timestampEnd = dataRecord.Child("timestamp_end").Value.ToString();
                        divesession.waterTemperatureMax = dataRecord.Child("water_temp_max").Value.ToString();
                        divesession.waterTemperatureMin = dataRecord.Child("water_temp_min").Value.ToString();
                        divesessionList.Add(divesession);
                    }
                    break;
                }
                default: { break; }
            }
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var records = snapshot.Children.ToEnumerable<DataSnapshot>();
                clearDataLists();

                createEntitiesFromSnapshot(snapshot);

                invokeDataRetrievedEvent(snapshot);
            }
            else 
            {
                DataRetrieved.Invoke(this, new DataEventArgs { Users = null, DiveSessions = null, Measurepoints = null });
            }
        }

        public void invokeDataRetrievedEvent(DataSnapshot snapshot) 
        {
            string tablename = snapshot.Ref.Key;

            if (tablename == "users")
            {
                DataRetrieved.Invoke(this, new DataEventArgs { Users = userList });
            }
            else if (tablename == "divesessions")
            {
                DataRetrieved.Invoke(this, new DataEventArgs { DiveSessions = divesessionList });
            }
            else if (tablename == "measurepoints") 
            {
                DataRetrieved.Invoke(this, new DataEventArgs { Measurepoints = measurePointList });
            }
        }

        public void clearDataLists() 
        {
            userList.Clear();
            divesessionList.Clear();
            measurePointList.Clear();
        }
    }
}