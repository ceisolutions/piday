using System;
using StackExchange.Redis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.Loader;

namespace sender
{

    class Program
    {
        //use a different port to send 'from' to prevent conflicts with the listner
        private const int BindPort = 11001;
        private const int SendPort = 11000;

        static ManualResetEvent _SIGTERM = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += obj => _SIGTERM.Set(); // Triggers the MRE when the user hits Ctrl-C or kills the proc

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            redis
                .GetSubscriber()
                .Subscribe("outgoing")
                .OnMessage(message => SendMessage(message.Message));

            Console.WriteLine("Press Ctrl-C to stop");
            _SIGTERM.WaitOne();
        }

        static void SendMessage(string message)
        {
            Console.WriteLine(message);
            try
            {
                var u = new UdpClient(BindPort) { EnableBroadcast = true };
                var ipAddress = IPAddress.Parse("192.168.2.255");
+               u.Connect(ipAddress, SendPort);

                byte[] sendbuf = Encoding.ASCII.GetBytes(message);

                u.Send(sendbuf, sendbuf.Length);
                u.Close();
                Console.WriteLine("Message sent to the broadcast address");
            }
            catch (Exception oops)
            {
                Console.WriteLine(oops.Message);
            }


        }
    }
}
