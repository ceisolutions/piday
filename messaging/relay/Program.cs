using System;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.Azure.Devices.Client;
using StackExchange.Redis;

namespace relay
{
    class Program
    {

        const string IOT_HUB_DEVICE_CONNECTION_STRING = "";
    
        static DeviceClient _Client;

        static ManualResetEvent _SIGTERM = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += obj => _SIGTERM.Set();
            
            _Client = DeviceClient.CreateFromConnectionString(IOT_HUB_DEVICE_CONNECTION_STRING, TransportType.Mqtt);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            redis
                .GetSubscriber()
                .Subscribe("incoming")
                .OnMessage(message => RelayMessage(message.Message));

            Console.WriteLine("Press Ctrl-C to stop.");
            _SIGTERM.WaitOne();
        }

        static void RelayMessage(string message) {
            Console.WriteLine("Relaying to IoT Hub: " + message);
             _Client.SendEventAsync(new Message(System.Text.Encoding.ASCII.GetBytes(message)));
        }
    }
}
