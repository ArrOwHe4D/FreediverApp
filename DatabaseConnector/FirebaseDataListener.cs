using Firebase.Database;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using DBConnector = FreediverApp.DatabaseConnector.DatabaseConnector;

namespace FreediverApp.DatabaseConnector
{
    /**
     *  This Class handles all db operations that are needed within our app. We use generic lists 
     *  that consist of the dataclasses we have designed (Dive, DiveSession, Measurepoint, User) to retrieve information
     *  from the db. We need to create an instance of this class in any Activity/Fragment that needs to communicate with the db.
     *  The lists of retrieved data records will then be returned to the Activity/Fragment by invoking the dataRetrieved Event 
     *  that has to be connected with the FirebaseDataListener instance.
     */
    public class FirebaseDataListener : Java.Lang.Object, IValueEventListener
    {
        //Generic Entity Lists to retreive the data results from db
        List<User> userList = new List<User>();
        List<DiveSession> divesessionList = new List<DiveSession>();
        List<Dive> diveList = new List<Dive>();
        List<Measurepoint> measurePointList = new List<Measurepoint>();

        //
        public event EventHandler<DataEventArgs> DataRetrieved;

        //EventArguments that hold our result lists and pass them to the Activity that triggered this event.
        public class DataEventArgs : EventArgs 
        {
            internal List<User> Users { get; set; }
            internal List<DiveSession> DiveSessions { get; set; }
            internal List<Measurepoint> Measurepoints { get; set; }
            internal List<Dive> Dives { get; set; }
        }

        public void QueryFullTable(string tablename) 
        {
            DatabaseReference tableRef = DBConnector.GetDatabase().GetReference(tablename);
            tableRef.AddValueEventListener(this);
        }

        //Queries a table "tablename" and filters the result with the given "field" and "value" like the WHERE clause in SQL.
        public void QueryParameterized(string tablename, string field, string value) 
        {
            DatabaseReference tableRef = DBConnector.GetDatabase().GetReference(tablename);
            tableRef.OrderByChild(field).EqualTo(value).AddValueEventListener(this);
        }




        public void OnCancelled(DatabaseError error)
        {
            
        }

        //Handles the results and instatiates a DataObject in our app for every dataset that was retrieved from db
        public void createEntitiesFromSnapshot(DataSnapshot snapshot) 
        {
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
                        divesession.location_lon = dataRecord.Child("location_lon").Value.ToString();
                        divesession.location_lat = dataRecord.Child("location_lat").Value.ToString();
                        divesession.refUser = dataRecord.Child("ref_user").Value.ToString();
                        divesession.watertime = dataRecord.Child("watertime").Value.ToString();
                        divesession.weatherCondition_main = dataRecord.Child("weather_condition_main").Value.ToString();
                        divesession.weatherCondition_description = dataRecord.Child("weather_condition_description").Value.ToString();
                        divesession.weatherTemperature = dataRecord.Child("weather_temp").Value.ToString();
                        divesession.weatherTemperatureFeelsLike = dataRecord.Child("weather_temp_feels_like").Value.ToString();
                        divesession.weatherPressure = dataRecord.Child("weather_pressure").Value.ToString();
                        divesession.weatherHumidity = dataRecord.Child("weather_humidity").Value.ToString();
                        divesession.weatherWindSpeed = dataRecord.Child("weather_wind_speed").Value.ToString();
                        divesession.weatherWindGust = dataRecord.Child("weather_wind_gust").Value.ToString();
                        divesession.Id = dataRecord.Child("id").Value.ToString();
                        divesessionList.Add(divesession);
                    }
                    break;
                }
                case "dives":
                {
                    foreach (DataSnapshot dataRecord in dataResult)
                    {
                        Dive dive = new Dive();                        
                        dive.duration = dataRecord.Child("duration").Value.ToString();
                        dive.HeartFreqMax = dataRecord.Child("heart_freq_max").Value.ToString();
                        dive.HeartFreqMin = dataRecord.Child("heart_freq_min").Value.ToString();
                        dive.LuminanceMax = dataRecord.Child("luminance_max").Value.ToString();
                        dive.LuminanceMin = dataRecord.Child("luminance_min").Value.ToString();
                        dive.maxDepth = dataRecord.Child("max_depth").Value.ToString();
                        dive.OxygenSaturationMax = dataRecord.Child("oxygen_saturation_max").Value.ToString();
                        dive.OxygenSaturationMin = dataRecord.Child("oxygen_saturation_min").Value.ToString();                           
                        dive.refDivesession = dataRecord.Child("ref_divesession").Value.ToString();
                        dive.timestampBegin = dataRecord.Child("timestamp_begin").Value.ToString();
                        dive.timestampEnd = dataRecord.Child("timestamp_end").Value.ToString();
                        dive.WaterTemperatureMax = dataRecord.Child("water_temp_max").Value.ToString();
                        dive.WaterTemperatureMin = dataRecord.Child("water_temp_min").Value.ToString();
                        dive.id = dataRecord.Child("id").Value.ToString();
                        diveList.Add(dive);
                    }
                    break;
                }
                case "measurepoints":
                {
                    foreach (DataSnapshot dataRecord in dataResult)
                    {
                        Measurepoint measurepoint = new Measurepoint();
                        measurepoint.accelerator_x = dataRecord.Child("accelerator_x").Value.ToString();
                        measurepoint.accelerator_y = dataRecord.Child("accelerator_y").Value.ToString();
                        measurepoint.accelerator_z = dataRecord.Child("accelerator_z").Value.ToString();
                        measurepoint.depth = dataRecord.Child("depth").Value.ToString();
                        measurepoint.duration = dataRecord.Child("duration").Value.ToString();
                        measurepoint.gyroscope_x = dataRecord.Child("gyroscope_x").Value.ToString();
                        measurepoint.gyroscope_y = dataRecord.Child("gyroscope_y").Value.ToString();
                        measurepoint.gyroscope_z = dataRecord.Child("gyroscope_z").Value.ToString();
                        measurepoint.heart_freq = dataRecord.Child("heart_freq").Value.ToString();
                        measurepoint.heart_var = dataRecord.Child("heart_var").Value.ToString();
						measurepoint.luminance = dataRecord.Child("luminance").Value.ToString();
                        measurepoint.oxygen_saturation = dataRecord.Child("oxygen_saturation").Value.ToString();
                        measurepoint.ref_dive = dataRecord.Child("ref_dive").Value.ToString();
                        measurepoint.water_temp = dataRecord.Child("water_temp").Value.ToString();
                        measurePointList.Add(measurepoint);
                    }
                    break;
                }
                default: { break; }
            }
        }

        //Stores the given "objectToSave" into the table "tablename" inside the database.
        public void saveEntity(string tablename, object objectToSave) 
        {
            HashMap saveData = new HashMap();
            DatabaseReference tableRef = DBConnector.GetDatabase().GetReference(tablename).Push();
            
            switch (tablename) 
            {
                case "users":
                {
                    var obj = (User)objectToSave;
                    saveData.Put("email", obj.email);
                    saveData.Put("username", obj.username);
                    saveData.Put("password", Encryptor.Encrypt(obj.password));
                    saveData.Put("firstname", obj.firstname);
                    saveData.Put("lastname", obj.lastname);
                    saveData.Put("birthday", obj.dateOfBirth);
                    saveData.Put("weight", obj.weight);
                    saveData.Put("height", obj.height);
                    saveData.Put("registerdate", DateTime.Now.Date.ToString("dd.MM.yyyy"));

                    tableRef.SetValue(saveData);
                    break;
                }
                case "dives": 
                {
                    var obj = (Dive)objectToSave;
                    saveData.Put("duration", obj.GetTotalTime());
                    saveData.Put("heart_freq_max", obj.HeartFreqMax);
                    saveData.Put("heart_freq_min", obj.HeartFreqMin);
                    saveData.Put("luminance_max", obj.LuminanceMax);
                    saveData.Put("luminance_min", obj.LuminanceMin);
                    saveData.Put("max_depth", obj.maxDepth);
                    saveData.Put("oxygen_saturation_max", obj.OxygenSaturationMax);
                    saveData.Put("oxygen_saturation_min", obj.OxygenSaturationMin);
                    saveData.Put("ref_divesession", obj.refDivesession);
                    saveData.Put("timestamp_begin", obj.timestampBegin);
                    saveData.Put("timestamp_end", obj.timestampEnd);
                    saveData.Put("water_temp_max", obj.WaterTemperatureMax);
                    saveData.Put("water_temp_min", obj.WaterTemperatureMin);
                    saveData.Put("id", obj.id);

                    tableRef.SetValue(saveData);
                    break;
                }
                case "divesessions":
                {
                    var obj = (DiveSession)objectToSave;
                    saveData.Put("date", obj.date);
                    saveData.Put("location_lon", obj.location_lon);
                    saveData.Put("location_lat", obj.location_lat);
                    saveData.Put("ref_user", obj.refUser);
                    saveData.Put("watertime", obj.watertime);
                    saveData.Put("weather_condition_main", obj.weatherCondition_main);
                    saveData.Put("weather_condition_description", obj.weatherCondition_description);
                    saveData.Put("weather_temp", obj.weatherTemperature);
                    saveData.Put("weather_temp_feels_like", obj.weatherTemperatureFeelsLike);
                    saveData.Put("weather_pressure", obj.weatherPressure);
                    saveData.Put("weather_humidity", obj.weatherHumidity);
                    saveData.Put("weather_wind_speed", obj.weatherWindSpeed);
                    saveData.Put("weather_wind_gust", obj.weatherWindGust);
                    saveData.Put("id", obj.Id);

                    tableRef.SetValue(saveData);
                    break;
                }
                case "measurepoints": 
                {
                    var obj = (Measurepoint)objectToSave;
                    saveData.Put("accelerator_x", obj.accelerator_x);
                    saveData.Put("accelerator_y", obj.accelerator_y);
                    saveData.Put("accelerator_z", obj.accelerator_z);
                    saveData.Put("depth", obj.depth);
                    saveData.Put("duration", obj.duration);
                    saveData.Put("gyroscope_x", obj.gyroscope_x);
                    saveData.Put("gyroscope_y", obj.gyroscope_y);
                    saveData.Put("gyroscope_z", obj.gyroscope_z);
                    saveData.Put("heart_freq", obj.heart_freq);
                    saveData.Put("heart_var", obj.heart_var);
                    saveData.Put("luminance", obj.luminance);
                    saveData.Put("oxygen_saturation", obj.oxygen_saturation);
                    saveData.Put("ref_dive", obj.ref_dive);
                    saveData.Put("water_temp", obj.water_temp);

                    tableRef.SetValue(saveData);
                    break;
                }
                default: 
                {
                    Console.WriteLine($"The table {tablename} does not exist!");
                    break;
                }
            }
        }


        //Deletes a dataset with the given "id" from the table "tablename"
        public void deleteEntity(string tablename, string id) 
        {
            DatabaseReference entityRef = DBConnector.GetDatabase().GetReference(tablename + "/" + id);
            entityRef.RemoveValue();
        }

        //Deletes a full table "tablename" from db
        public void deleteTable(string tablename) 
        {
            DatabaseReference tableRef = DBConnector.GetDatabase().GetReference(tablename);
            tableRef.RemoveValue();
        }

        //Event that is called when data was received from db
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
                DataRetrieved.Invoke(this, new DataEventArgs { Users = null, DiveSessions = null, Dives = null, Measurepoints = null });
            }
        }

        //Event that is invoked to store our result lists inside our eventhandler with the DataEventArgs. 
        public void invokeDataRetrievedEvent(DataSnapshot snapshot) 
        {
            string tablename = snapshot.Ref.Key;

            if (tablename == "users")
            {
                DataRetrieved.Invoke(this, new DataEventArgs { Users = userList, DiveSessions = null, Dives = null, Measurepoints = null });
            }
            else if (tablename == "divesessions")
            {
                DataRetrieved.Invoke(this, new DataEventArgs { DiveSessions = divesessionList, Users = null, Dives = null, Measurepoints = null });
            }
            else if (tablename == "dives") 
            {
                DataRetrieved.Invoke(this, new DataEventArgs { Dives = diveList, Users = null, DiveSessions = null, Measurepoints = null });
            }
            else if (tablename == "measurepoints")
            {
                DataRetrieved.Invoke(this, new DataEventArgs { Measurepoints = measurePointList, Users = null, DiveSessions = null, Dives = null });
            }
        }

        public void clearDataLists() 
        {
            userList.Clear();
            divesessionList.Clear();
            diveList.Clear();
            measurePointList.Clear();
        }
    }
}