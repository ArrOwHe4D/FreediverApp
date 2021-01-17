using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;

//DataListener that handles the retrieval of data from db and returns the data to the activity/fragment by invoking a event
namespace FreediverApp.DatabaseConnector
{
    public class FirebaseDataListener : Java.Lang.Object, IValueEventListener
    {
        //Lists for retrieved data entities that will then be returned to the activity by invoking the dataRetrieved Event
        //If we need a new type of table you should add a new list to handle the retrieved Data for that case
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
            //we cannot generalize this completely at the moment -> would be too much since we dont need a full db api for the small amount of tasks
            string tablename = snapshot.Ref.Key;
            var dataResult = snapshot.Children.ToEnumerable<DataSnapshot>();

            //determine which table was queried. If another table needs to be handled the case has to be added to the below switch statement
            switch (tablename)
            {
                case "users":
                {
                    foreach (DataSnapshot dataRecord in dataResult)
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
                        user.registerdate = dataRecord.Child("registerdate").Value.ToString();
                        userList.Add(user);
                    }
                    break;
                }
                case "divesessions":
                {
                    foreach (DataSnapshot dataRecord in dataResult)
                    {
                        DiveSession divesession = new DiveSession();
                        divesession.date = dataRecord.Child("date").Value.ToString();
                        divesession.duration = dataRecord.Child("duration").Value.ToString();
                        divesession.HeartFreqMax = dataRecord.Child("heart_freq_max").Value.ToString();
                        divesession.HeartFreqMin = dataRecord.Child("heart_freq_min").Value.ToString();
                        divesession.LuminanceMax = dataRecord.Child("luminance_max").Value.ToString();
                        divesession.LuminanceMin = dataRecord.Child("luminance_min").Value.ToString();
                        divesession.maxDepth = dataRecord.Child("max_depth").Value.ToString();
                        divesession.OxygenSaturationMax = dataRecord.Child("oxygen_saturation_max").Value.ToString();
                        divesession.OxygenSaturationMin = dataRecord.Child("oxygen_saturation_min").Value.ToString();
                        divesession.refUser = dataRecord.Child("ref_user").Value.ToString();
                        divesession.timestampBegin = dataRecord.Child("timestamp_begin").Value.ToString();
                        divesession.timestampEnd = dataRecord.Child("timestamp_end").Value.ToString();
                        divesession.WaterTemperatureMax = dataRecord.Child("water_temp_max").Value.ToString();
                        divesession.WaterTemperatureMin = dataRecord.Child("water_temp_min").Value.ToString();
                        divesessionList.Add(divesession);
                    }
                    break;
                }
                case "measurepoints":
                {
                    foreach (DataSnapshot dataRecord in dataResult)
                    {
                        Measurepoint measurepoint = new Measurepoint();
                        measurepoint.acceleration = dataRecord.Child("acceleration").Value.ToString();
                        measurepoint.depth = dataRecord.Child("depth").Value.ToString();
                        measurepoint.duration = dataRecord.Child("duration").Value.ToString();
                        measurepoint.gyroscope = dataRecord.Child("gyroscope").Value.ToString();
                        measurepoint.heartFreq = dataRecord.Child("heart_freq").Value.ToString();
                        measurepoint.heartVar = dataRecord.Child("heart_var").Value.ToString();
                        measurepoint.luminance = dataRecord.Child("luminance").Value.ToString();
                        measurepoint.magnetSensorData = dataRecord.Child("magnet_sensor_data").Value.ToString();
                        measurepoint.oxygenSaturation = dataRecord.Child("oxygen_saturation").Value.ToString();
                        measurepoint.refDivesession = dataRecord.Child("ref_divesession").Value.ToString();
                        measurepoint.waterTemperature = dataRecord.Child("water_temp").Value.ToString();
                        measurePointList.Add(measurepoint);
                    }
                    break;
                }
                default: { break; }
            }
        }

        public void deleteEntity(string tablename, string id) 
        {
            DatabaseReference entityRef = DatabaseConnector.GetDatabase().GetReference(tablename + "/" + id);
            entityRef.RemoveValue();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                //clear all datalists before new retrieved data is loaded from db
                clearDataLists();

                //instantiate a dataobject for every entry that was returned by the query
                createEntitiesFromSnapshot(snapshot);

                //invoke the dataRetrievedEvent for the corresponding table
                invokeDataRetrievedEvent(snapshot);
            }
            else 
            {
                //if no data was returned also invoke the dataRetrievedEvent but set all datalists to null
                DataRetrieved.Invoke(this, new DataEventArgs { Users = null, DiveSessions = null, Measurepoints = null });
            }
        }

        public void invokeDataRetrievedEvent(DataSnapshot snapshot) 
        {
            string tablename = snapshot.Ref.Key;

            if (tablename == "users")
            {
                DataRetrieved.Invoke(this, new DataEventArgs { Users = userList, DiveSessions = null, Measurepoints = null });
            }
            else if (tablename == "divesessions")
            {
                DataRetrieved.Invoke(this, new DataEventArgs { DiveSessions = divesessionList, Users = null, Measurepoints = null });
            }
            else if (tablename == "measurepoints") 
            {
                DataRetrieved.Invoke(this, new DataEventArgs { Measurepoints = measurePointList, Users = null, DiveSessions = null });
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