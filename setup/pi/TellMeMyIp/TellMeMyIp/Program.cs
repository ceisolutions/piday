using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace TellMeMyIp
{
    class Program
    {
        private static int WaitBetweenMessages = 5000;

        static void Main(string[] args)
        {
            var messages = new List<string>();

            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in adapters)
            {
                var properties = adapter.GetIPProperties();
                foreach (var address in properties.UnicastAddresses)
                {
                    if (adapter.OperationalStatus == OperationalStatus.Up &&
                        address.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !address.Equals(IPAddress.Loopback))
                    {
                        Console.WriteLine($"Found: {address.Address.ToString()}");
                        messages.Add(address.Address.ToString());
                    }
                }
            }

            DisplayMessages(messages);
        }

        private static void DisplayMessages(List<string> messages)
        {
            while (true)
            {
                foreach (var message in messages)
                {
                    Console.WriteLine($"RGB Display: {message}");
                    Sense.Led.LedMatrix.ShowMessage(message);
                    Thread.Sleep(WaitBetweenMessages);
                }
            }
        }
    }
}
