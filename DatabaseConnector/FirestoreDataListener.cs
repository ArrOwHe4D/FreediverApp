using Android.Gms.Tasks;
using Firebase.Firestore;
using FreediverApp.DataClasses;
using Java.Util;
using System;
using System.Collections.Generic;
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
    public class FirestoreDataListener : Java.Lang.Object, IOnSuccessListener
    {
        //Generic Entity Lists to retreive the data results from db
        List<User> userList = new List<User>();
        List<DiveSession> divesessionList = new List<DiveSession>();
        List<Dive> diveList = new List<Dive>();
        List<Measurepoint> measurePointList = new List<Measurepoint>();
        List<SavedSession> savedSessionsList = new List<SavedSession>();
        string lastQueriedTableName;

        //Event that is invoked when new data was received from the db
        public event EventHandler<DataEventArgs> DataRetrieved;

        //EventArguments that hold our result lists and pass them to the Activity that triggered this event.
        public class DataEventArgs : EventArgs
        {
            internal List<User> Users { get; set; }
            internal List<DiveSession> DiveSessions { get; set; }
            internal List<Measurepoint> Measurepoints { get; set; }
            internal List<Dive> Dives { get; set; }
            internal List<SavedSession> SavedSessions { get; set; }
        }

        //Queries a full table returning all entities that belong to that table
        public void QueryFullTable(string tablename)
        {
            lastQueriedTableName = tablename;
            DBConnector.GetFirestoreDatabase().Collection(tablename)
                .Get().AddOnSuccessListener(this);
        }

        //Queries a table "tablename" and filters the result with the given "field" and "value" like the WHERE clause in SQL.
        public void QueryParameterized(string tablename, string field, string value)
        {
            lastQueriedTableName = tablename;
            DBConnector.GetFirestoreDatabase().Collection(tablename)
                .WhereEqualTo(field, value)
                .Get().AddOnSuccessListener(this);
        }

        public void QueryParameterizedMulti(string tablename, string field, string value, string field2, string value2)
        {
            lastQueriedTableName = tablename;
            DBConnector.GetFirestoreDatabase().Collection(tablename)
                .WhereEqualTo(field, value)
                .WhereEqualTo(field2, value2)
                .Get().AddOnSuccessListener(this);
        }

        //Updates the value of the given "field" from a dataset with the given "id" with the parameter "value"
        public void updateEntity(string tablename, string id, string field, string value)
        {
            DocumentReference entityRef = DBConnector.GetFirestoreDatabase().Collection(tablename).Document(id);
            entityRef.Update(field, value);
        }

        //Deletes a dataset with the given "id" from the table "tablename"
        public void deleteEntity(string tablename, string id)
        {
            DocumentReference entityRef = DBConnector.GetFirestoreDatabase().Collection(tablename).Document(id);
            entityRef.Delete();
        }

        //Handles the results and instatiates a DataObject in our app for every dataset that was retrieved from db
        private void createEntitiesFromSnapshot(QuerySnapshot snapshot)
        {
            var dataResult = snapshot.Documents;

            //determine which table was queried. If another table needs to be handled the case has to be added to the below switch statement
            switch (lastQueriedTableName)
            {
                case "users":
                {
                    foreach (DocumentSnapshot dataRecord in dataResult)
                    {
                        User user                                = new User();
                        user.id                                  = dataRecord.Id;
                        user.username                            = dataRecord.GetString("username");
                        user.password                            = dataRecord.GetString("password");
                        user.email                               = dataRecord.GetString("email");
                        user.firstname                           = dataRecord.GetString("firstname");
                        user.lastname                            = dataRecord.GetString("lastname");
                        user.dateOfBirth                         = dataRecord.GetString("birthday");
                        user.weight                              = dataRecord.GetString("weight");
                        user.height                              = dataRecord.GetString("height");
                        user.registerdate                        = dataRecord.GetString("registerdate");

                        userList.Add(user);
                    }
                    break;
                }
                case "divesessions":
                {
                    foreach (DocumentSnapshot dataRecord in dataResult)
                    {
                        DiveSession divesession                  = new DiveSession();
                        divesession.key                          = dataRecord.Id;
                        divesession.date                         = dataRecord.GetString("date");
                        divesession.location_lon                 = dataRecord.GetString("location_lon");
                        divesession.location_lat                 = dataRecord.GetString("location_lat");
                        divesession.refUser                      = dataRecord.GetString("ref_user");
                        divesession.watertime                    = dataRecord.GetString("watertime");
                        divesession.weatherCondition_main        = dataRecord.GetString("weather_condition_main");
                        divesession.weatherCondition_description = dataRecord.GetString("weather_condition_description");
                        divesession.weatherTemperature           = dataRecord.GetString("weather_temp");
                        divesession.weatherTemperatureFeelsLike  = dataRecord.GetString("weather_temp_feels_like");
                        divesession.weatherPressure              = dataRecord.GetString("weather_pressure");
                        divesession.weatherHumidity              = dataRecord.GetString("weather_humidity");
                        divesession.weatherWindSpeed             = dataRecord.GetString("weather_wind_speed");
                        divesession.weatherWindGust              = dataRecord.GetString("weather_wind_gust");
                        divesession.note                         = dataRecord.GetString("note");
                        divesession.Id                           = dataRecord.GetString("id");

                        divesessionList.Add(divesession);
                    }
                    break;
                }
                case "dives":
                {
                    foreach (DocumentSnapshot dataRecord in dataResult)
                    {
                        Dive dive                                = new Dive();
                        dive.duration                            = dataRecord.GetString("duration");
                        dive.HeartFreqMax                        = dataRecord.GetString("heart_freq_max");
                        dive.HeartFreqMin                        = dataRecord.GetString("heart_freq_min");
                        dive.LuminanceMax                        = dataRecord.GetString("luminance_max");
                        dive.LuminanceMin                        = dataRecord.GetString("luminance_min");
                        dive.maxDepth                            = dataRecord.GetString("max_depth");
                        dive.OxygenSaturationMax                 = dataRecord.GetString("oxygen_saturation_max");
                        dive.OxygenSaturationMin                 = dataRecord.GetString("oxygen_saturation_min");
                        dive.refDivesession                      = dataRecord.GetString("ref_divesession");
                        dive.timestampBegin                      = dataRecord.GetString("timestamp_begin");
                        dive.timestampEnd                        = dataRecord.GetString("timestamp_end");
                        dive.WaterTemperatureMax                 = dataRecord.GetString("water_temp_max");
                        dive.WaterTemperatureMin                 = dataRecord.GetString("water_temp_min");
                        dive.id                                  = dataRecord.GetString("id");

                        diveList.Add(dive);
                    }
                    break;
                }
                case "measurepoints":
                {
                    foreach (DocumentSnapshot dataRecord in dataResult)
                    {
                        Measurepoint measurepoint                = new Measurepoint();
                        measurepoint.accelerator_x               = dataRecord.GetString("accelerator_x");
                        measurepoint.accelerator_y               = dataRecord.GetString("accelerator_y");
                        measurepoint.accelerator_z               = dataRecord.GetString("accelerator_z");
                        measurepoint.depth                       = dataRecord.GetString("depth");
                        measurepoint.duration                    = dataRecord.GetString("duration");
                        measurepoint.gyroscope_x                 = dataRecord.GetString("gyroscope_x");
                        measurepoint.gyroscope_y                 = dataRecord.GetString("gyroscope_y");
                        measurepoint.gyroscope_z                 = dataRecord.GetString("gyroscope_z");
                        measurepoint.heart_freq                  = dataRecord.GetString("heart_freq");
                        measurepoint.heart_var                   = dataRecord.GetString("heart_var");
                        measurepoint.luminance                   = dataRecord.GetString("luminance");
                        measurepoint.oxygen_saturation           = dataRecord.GetString("oxygen_saturation");
                        measurepoint.ref_dive                    = dataRecord.GetString("ref_dive");
                        measurepoint.water_temp                  = dataRecord.GetString("water_temp");

                        measurePointList.Add(measurepoint);
                    }
                    break;
                }
                case "savedsessions":
                {
                    foreach (DocumentSnapshot dataRecord in dataResult)
                    {
                        SavedSession savedsessions               = new SavedSession();
                        savedsessions.ref_user                   = dataRecord.GetString("ref_user");
                        savedsessions.sessiondate                = dataRecord.GetString("sessiondate");
                        savedSessionsList.Add(savedsessions);
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
            DocumentReference documentRef = DBConnector.GetFirestoreDatabase().Collection(tablename).Document();

            switch (tablename)
            {
                case "users":
                {
                    var obj = (User)objectToSave;
                    saveData.Put("email", obj.email);
                    saveData.Put("username", obj.username);
                    saveData.Put("password", CryptoService.Encrypt(obj.password));
                    saveData.Put("firstname", obj.firstname);
                    saveData.Put("lastname", obj.lastname);
                    saveData.Put("birthday", obj.dateOfBirth);
                    saveData.Put("weight", obj.weight);
                    saveData.Put("height", obj.height);
                    saveData.Put("registerdate", DateTime.Now.Date.ToString("dd.MM.yyyy"));

                    documentRef.Set(saveData);
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
                    saveData.Put("timestamp_end", "n/a");
                    saveData.Put("water_temp_max", obj.WaterTemperatureMax);
                    saveData.Put("water_temp_min", obj.WaterTemperatureMin);
                    saveData.Put("id", obj.id);

                    documentRef.Set(saveData);
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
                    saveData.Put("note", "");
                    saveData.Put("id", obj.Id);

                    documentRef.Set(saveData);
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

                    documentRef.Set(saveData);
                    break;
                }
                case "savedsessions":
                {
                    var obj = (SavedSession)objectToSave;
                    saveData.Put("ref_user", obj.ref_user);
                    saveData.Put("sessiondate", obj.sessiondate);

                    documentRef.Set(saveData);
                    break;
                }
                default:
                {
                    Console.WriteLine($"The table {tablename} does not exist!");
                    break;
                }
            }
        }

        //Event that is invoked to store our result lists inside our eventhandler with the DataEventArgs. 
        private void invokeDataRetrievedEvent(QuerySnapshot snapshot)
        {
            DataRetrieved.Invoke(this, new DataEventArgs { Users = userList, DiveSessions = divesessionList, Dives = diveList, Measurepoints = measurePointList, SavedSessions = savedSessionsList }); 
        }

        private void clearDataLists()
        {
            userList.Clear();
            divesessionList.Clear();
            diveList.Clear();
            measurePointList.Clear();
            savedSessionsList.Clear();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            QuerySnapshot snapshot = (QuerySnapshot) result;

            if (snapshot != null && !snapshot.IsEmpty)
            {
                clearDataLists();

                createEntitiesFromSnapshot(snapshot);

                invokeDataRetrievedEvent(snapshot);
            }
            else 
            {
                //On Error, invoke the event with empty data to get a response inside the activity that uses the event listener
                DataRetrieved.Invoke(this, new DataEventArgs { Users = null, DiveSessions = null, Dives = null, Measurepoints = null, SavedSessions = null });
            }
        }
    }
}