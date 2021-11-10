using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Fragment = Android.App.Fragment;
using SupportV7 = Android.Support.V7.App;
using FreediverApp.DatabaseConnector;
using Newtonsoft.Json;
using Android.App;
using FreediverApp.WifiCommunication;
using Android.Content;
using System.IO;
using FreediverApp.DataClasses;
using FreediverApp.Utils;
using System.Threading;
using Xamarin.Essentials;

namespace FreediverApp
{
    [Obsolete]
    public class ConnectiveDevicesFragment : Fragment
    {
        /*Member Variables including UI components from XML and all needed BLE components*/
        private Button buttonTransfer;
        private Button buttonConnect;
        private Button buttonSync;
        private Button buttonSyncOta;
        private FirestoreDataListener diveSessionListener;
        private List<DiveSession> diveSessionsFromDatabase;
        private ProgressDialog dataTransferDialog;
        private WifiConnector wifiConnector;
        private FirestoreDataListener database;

        [Obsolete]
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        /**
         *  This function initializes all UI and BLE components. 
         **/
        [Obsolete]
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ConnectiveDevicesPage, container, false);

            //setup wifi connector
            wifiConnector = new WifiConnector(Context);

            //setup firebase data listener
            database = new FirestoreDataListener();

            buttonTransfer = view.FindViewById<Button>(Resource.Id.bt_scan_btn);

            //setup onclick listeners for scan button and listview items
            buttonTransfer.Click += buttonTransferOnClick;

            buttonConnect = view.FindViewById<Button>(Resource.Id.button_connect);
            buttonConnect.Click += buttonConnectOnClick;

            buttonSync = view.FindViewById<Button>(Resource.Id.button_sync);
            buttonSync.Click += buttonSyncOnClick;

            buttonSyncOta = view.FindViewById<Button>(Resource.Id.button_sync_ota);
            buttonSyncOta.Click += buttonSyncOtaOnClick;

            //setup the db listener to be able to query data from firebase
            diveSessionListener = new FirestoreDataListener();

            //query all divesessions of the current user since we need to determine if a session already exists when receiving data from arduino

            retrieveDiveSessions();
            checkWifiPermission();

            if (!wifiConnector.IsWifiEnabled()) 
            {
                runWifiActivationDialog();
            }
            return view;
        }

        private void buttonSyncOnClick(object sender, EventArgs eventArgs)
        {
            if (FreediverHelper.isConnectedToDatabase())
            {
                //Show data transfer dialog
                dataTransferDialog = new ProgressDialog(base.Context);
                dataTransferDialog.SetMessage(base.Context.Resources.GetString(Resource.String.dialog_upload_data_to_cloud));
                dataTransferDialog.SetCancelable(false);
                dataTransferDialog.Show();

                //Start Data Synchronization in background thread
                ThreadPool.QueueUserWorkItem(o => uploadDataTask());
            }
            else
            {
                Toast.MakeText(Context, Resource.String.no_internet_connection, ToastLength.Long).Show();
            }
        }

        private void buttonSyncOtaOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                //OTA UPDATE STUFF
                FtpConnector connector = new FtpConnector(base.Context, "192.168.4.1", "diver", "diverpass");               

                if (connector.isConnected())
                {
                    //Show data transfer dialog
                    dataTransferDialog = new ProgressDialog(base.Context);
                    dataTransferDialog.SetMessage(base.Context.Resources.GetString(Resource.String.dialog_sync_ota));
                    dataTransferDialog.SetCancelable(false);
                    dataTransferDialog.Show();

                    //Start Data Synchronization in background thread
                    ThreadPool.QueueUserWorkItem(o => synchronizeOtaDataTask(connector));
                }
                else
                {
                    Toast.MakeText(base.Context, Resource.String.connection_to_device_failed_ensure_wifi_connection, ToastLength.Long).Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Toast.MakeText(Context, Resource.String.connection_to_device_failed_ensure_wifi_connection, ToastLength.Long);
            }
        }

        private void uploadDataTask() 
        {
            int sessionCount = 0;
            while (true)
            {
                var session = readSessionFromFile();
                if (session.Key == null || session.Value == null)
                {
                    break;
                }
                sessionCount++;
                saveSessionData(session);    
            }

            //Close data transfer dialog after completion
            Activity.RunOnUiThread(() => dataTransferDialog.Dismiss());

            if (sessionCount > 0) 
            {
                Activity.RunOnUiThread(() => Toast.MakeText(Context, Resource.String.data_sync_complete, ToastLength.Long).Show());
            }
        }

        private void buttonConnectOnClick(object sender, EventArgs eventArgs) 
        {
            // Switch to native Android wifi Menu
            StartActivity(new Intent(Android.Provider.Settings.ActionWifiSettings));
        }

        /**
         *  This function initiates the scan process to find bluetooth devices in near range.
         *  It is defined as async so we wait for a scan result asynchronously and add all found devices
         *  to our list that is passed to the listview in form of a adapter component.
         **/
        private void buttonTransferOnClick(object sender, EventArgs eventArgs)
        {
            requestStoragePermissions();
            Toast.MakeText(base.Context, Resource.String.dialog_connecting_to_dive_computer, ToastLength.Long).Show();

            try
            {
                //DO FTP STUFF HERE - The connector immediately tries to connect to the given host after it is constructed
                FtpConnector connector = new FtpConnector(base.Context, "192.168.4.1", "diver", "diverpass");

                if (connector.isConnected())
                {
                    //Show data transfer dialog
                    dataTransferDialog = new ProgressDialog(base.Context);
                    dataTransferDialog.SetMessage(base.Context.Resources.GetString(Resource.String.dialog_receiving_data));
                    dataTransferDialog.SetCancelable(false);
                    dataTransferDialog.Show();

                    //Start Data Synchronization in background thread
                    ThreadPool.QueueUserWorkItem(o => synchronizeDataTask(connector));
                }
                else
                {
                    Toast.MakeText(base.Context, Resource.String.connection_to_device_failed_ensure_wifi_connection, ToastLength.Long).Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Toast.MakeText(Context, Resource.String.connection_to_device_failed_ensure_wifi_connection, ToastLength.Long);
            }
        }

        private void synchronizeDataTask(FtpConnector connector) 
        {
            //Attempt to sync data and start parsing it afterwards
            Activity.RunOnUiThread(() => Toast.MakeText(base.Context, Resource.String.connected_to_device_starting_data_transfer, ToastLength.Long).Show());
            Console.WriteLine("Starting to sync log directory...");
            DownloadReport downloadReport = connector.synchronizeData();
            saveDownloadReport(downloadReport);

            //Close data transfer dialog after completion
            Activity.RunOnUiThread(() => dataTransferDialog.Dismiss());
            Activity.RunOnUiThread(() => Toast.MakeText(Context, Resource.String.data_transfer_complete, ToastLength.Long).Show());
            Activity.RunOnUiThread(() => createPopupDialog());
        }

        private void synchronizeOtaDataTask(FtpConnector connector)
        {
            //Attempt to sync data and start parsing it afterwards
            Activity.RunOnUiThread(() => Toast.MakeText(base.Context, Resource.String.connected_to_device_starting_data_transfer, ToastLength.Long).Show());
            Console.WriteLine("Starting to sync OTA Data...");
            connector.synchronizeOtaData();

            //Close data transfer dialog after completion
            Activity.RunOnUiThread(() => dataTransferDialog.Dismiss());
            Activity.RunOnUiThread(() => Toast.MakeText(Context, Resource.String.data_transfer_complete, ToastLength.Long).Show());
        }

        private void saveDownloadReport(DownloadReport downloadReport) 
        {
            string filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/pending_sessions.ps";

            using (StreamWriter sr = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate)))
            {
                foreach (var session in downloadReport.getFilesToDownload())
                {
                    string sessionAsJson = JsonConvert.SerializeObject(session);
                    Console.WriteLine(sessionAsJson);
                    sr.WriteLine(sessionAsJson);
                }
            }
        }

        private KeyValuePair<string, List<string>> readSessionFromFile() 
        {
            string filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/pending_sessions.ps";          

            if (!File.Exists(filePath))
            {
                Activity.RunOnUiThread(() => Toast.MakeText(Context, Resource.String.no_pending_data, ToastLength.Long).Show());
                return new KeyValuePair<string, List<string>>(null, null);
            }

            List<string> sessions = File.ReadLines(filePath).ToList();

            if (sessions.Count > 0)
            {
                int lastSessionIndex = sessions.Count - 1;

                //get last session as result
                string result = sessions.ElementAt(lastSessionIndex);

                //remove last session to mark it as read
                sessions.RemoveAt(lastSessionIndex);

                //write all remaining sessions back to the file (overwrite)
                File.WriteAllLines(filePath, sessions);

                KeyValuePair<string, List<string>> jsonSessionObject = JsonConvert.DeserializeObject<KeyValuePair<string, List<string>>>(result);

                return jsonSessionObject;
            }
            else 
            {
                Activity.RunOnUiThread(() => Toast.MakeText(Context, Resource.String.no_pending_data, ToastLength.Long).Show());
            }
            return new KeyValuePair<string, List<string>>(null, null);
        }

        private void saveSessionData(KeyValuePair<string, List<string>> session) 
        {
            DiveSession diveSession = FileParser.parseSession(session);
            string existingSessionDate = getRelatedExistingSessionDate(diveSession);

            //save all measurepoints and all dives from the current parsed session to db
            foreach (Dive dive in diveSession.dives)
            {
                foreach (Measurepoint measurepoint in dive.measurepoints)
                {
                    database.saveEntity("measurepoints", measurepoint);
                }
                database.saveEntity("dives", dive);
            }
            
            //if the session not exists in db, set the divesession data to empty strings but save 
            //it in db anyway without weather and location data so that we don´t loose any collected data from arduino side
            if (existingSessionDate != diveSession.date)
            {
                diveSession.location_lat = "";
                diveSession.location_lon = "";
                diveSession.weatherCondition_description = "";
                diveSession.weatherCondition_main = "";
                diveSession.weatherHumidity = "";
                diveSession.weatherPressure = "";
                diveSession.weatherTemperature = "";
                diveSession.weatherTemperatureFeelsLike = "";
                diveSession.weatherWindGust = "";
                diveSession.weatherWindSpeed = "";
                diveSession.note = "-";
                diveSession.UpdateAll(); //maybe not neccessary
                database.saveEntity("divesessions", diveSession);
                database.saveEntity("savedsessions", new SavedSession(TemporaryData.CURRENT_USER.id, diveSession.date));
            }
        }

        private void checkWifiPermission()
        {
            const int wifiPermissionsRequestCode = 1000;

            var wifiPermissions = new[]
            {
                Manifest.Permission.AccessWifiState,
                Manifest.Permission.ChangeWifiState
            };

            var wifiStatePermissionGranted = ContextCompat.CheckSelfPermission(Context, wifiPermissions[0]);

            var wifiStateChangePermissionGranted = ContextCompat.CheckSelfPermission(Context, wifiPermissions[1]);

            if (wifiStatePermissionGranted == Permission.Denied || wifiStateChangePermissionGranted == Permission.Denied)
                ActivityCompat.RequestPermissions(Activity, wifiPermissions, wifiPermissionsRequestCode);
        }

        private void runWifiActivationDialog()
        {
            //setup UI for the activation dialog
            SupportV7.AlertDialog.Builder wifiActivationDialog = new SupportV7.AlertDialog.Builder(Context);
            wifiActivationDialog.SetTitle(base.Context.Resources.GetString(Resource.String.dialog_wifi_not_activated));
            wifiActivationDialog.SetMessage(base.Context.Resources.GetString(Resource.String.dialog_want_to_activate_wifi));

            wifiActivationDialog.SetPositiveButton(Resource.String.dialog_accept, (senderAlert, args) =>
            {
                //lambda expression that handles the case that the user accepted to activate wifi.
                //wifiConnector.SetWifiEnabled(true);
                StartActivity(new Intent(Android.Provider.Settings.ActionWifiSettings));

                //print a toast message whether or not the bt adapter was sucessfully activated
                if (wifiConnector.IsWifiEnabled())
                {
                    Toast.MakeText(Context, Resource.String.wifi_activation_successful, ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(Context, Resource.String.wifi_activation_failed, ToastLength.Long).Show();
                }
            });
            wifiActivationDialog.SetNegativeButton(Resource.String.dialog_cancel, (senderAlert, args) =>
            {
                //close dialog on cancel
                wifiActivationDialog.Dispose();
            });

            wifiActivationDialog.Show();
        }

        //Check if a divesession already exists in db and set the ref_divesession field to
        //the id of the existing divesession to realize the 1:n relation of divession and dives
        private string getRelatedExistingSessionDate(DiveSession diveSession)
        {
            //Store dates of existing sessions and setup a new listener to save Dives and Measurepoints after 
            //creating the correct reference to a existing divesession
            string existingSessionDate = "";

            if (diveSessionsFromDatabase != null)
            {
                foreach (DiveSession savedSession in diveSessionsFromDatabase)
                {
                    if (savedSession.date == diveSession.date)
                    {
                        existingSessionDate = diveSession.date;
                        foreach (Dive dive in diveSession.dives)
                        {
                            dive.refDivesession = savedSession.Id;
                        }
                        int watertime = Convert.ToInt32(savedSession.watertime) + Convert.ToInt32(diveSession.watertime);
                        database.updateEntity("divesessions", savedSession.key, "watertime", watertime.ToString());
                    }
                }
            }
            return existingSessionDate;
        }

        /**
         *  This function queries for all divesessions that were created by the current user. We need
         *  this query to determine if a session already exists to connect the data read from arduino side with
         *  the already existing sessions. 
         **/
        private void retrieveDiveSessions()
        {
            diveSessionListener.QueryParameterized("divesessions", "ref_user", TemporaryData.CURRENT_USER.id);
            diveSessionListener.DataRetrieved += database_diveSessionDataRetrieved;
        }

        /**
         *  This function is called when divesession data was received by the db listener to check for existing sessions inside the list
         *  that is being set by this function. 
         **/
        private void database_diveSessionDataRetrieved(object sender, FirestoreDataListener.DataEventArgs args)
        {
            diveSessionsFromDatabase = args.DiveSessions;
        }

        void createPopupDialog() 
        {
            LayoutInflater layoutInflater = LayoutInflater.From(Context);
            View dialogView = layoutInflater.Inflate(Resource.Layout.PopupDialog, null);

            SupportV7.AlertDialog.Builder dialogBuilder = createPopupDialog("Info", base.Context.Resources.GetString(Resource.String.dialog_disconnect_and_establish_internet_connection_to_sync), Resource.Drawable.icon_info, dialogView);

            dialogBuilder.SetCancelable(false)
                .SetPositiveButton("OK", delegate
                {
                    dialogBuilder.Dispose();
                });

            SupportV7.AlertDialog dialog = dialogBuilder.Create();
            dialog.Show();
        }

        public SupportV7.AlertDialog.Builder createPopupDialog(string title, string messageText, int iconId, View parentView)
        {
            SupportV7.AlertDialog.Builder dialogBuilder = new SupportV7.AlertDialog.Builder(Context);
            dialogBuilder.SetView(parentView);

            var textViewMessage = parentView.FindViewById<TextView>(Resource.Id.textview_message);
            textViewMessage.Text = messageText;

            dialogBuilder.SetTitle(title);
            dialogBuilder.SetIcon(iconId);
                   
            return dialogBuilder;
        }

        private void requestStoragePermissions() 
        {
            if ((ContextCompat.CheckSelfPermission(Context, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            || (ContextCompat.CheckSelfPermission(Context, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted))
            {
                ActivityCompat.RequestPermissions(Activity, new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, 1000);
            }
        }
    }
}
