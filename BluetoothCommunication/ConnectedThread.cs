using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.Bluetooth;
using System.IO;

namespace FreediverApp.BluetoothCommunication
{
    public class ConnectedThread
    {
        private BluetoothSocket btSocket;
        private Stream btReader;
        private Stream btWriter;

        public ConnectedThread(BluetoothSocket socket)
        {
            btSocket = socket;
            Stream tmpReader = null;
            Stream tmpWriter = null;

            try
            {
                tmpReader = socket.InputStream;
                tmpWriter = socket.OutputStream;
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }

            btReader = tmpReader;
            btWriter = tmpWriter;
        }



        public void run()
        {
            byte[] buffer = new byte[1024];
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = btReader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    buffer = ms.ToArray();
                }
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }
            

        }

    }
}

