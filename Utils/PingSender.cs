using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace StreamerShutdown
{
    class PingSender
    {
        Ping ping;
        PingOptions pingOptions; 
        PingReply reply;

        string data;
        byte[] buffer;
        int timeout;

        public PingSender()
        {
            ping = new Ping();
            pingOptions = new PingOptions();
            pingOptions.DontFragment = true;
            data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            buffer = Encoding.ASCII.GetBytes(data);
            timeout = 120;
        }

        public void sendPing()
        {
            try
            {
                reply = ping.Send("www.twitch.tv", timeout, buffer, pingOptions);
            } catch(Exception exp) { }

            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("Address: {0}", reply.Address.ToString());
                Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
            }
            
        }

        public void SendPing()
        {
            while (true)
            {
                sendPing();
            }
        }
    }
}
