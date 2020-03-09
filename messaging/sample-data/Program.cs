using System;
using StackExchange.Redis;
using System.Threading;

namespace sample_data
{
    class Program
    {
        static string[] ROOMS = { "living-room", "dining-room", "family-room", "bedroom" };
        static string[] SENSORS = { "temperature", "pressure", "humidity" };
        static string[] CITIES = { "Pittsburgh", "Cleveland", "Rochester", "Detroit" };
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            Console.WriteLine("Generating sample data, Ctrl-C to stop");
            while (true)
            {
                Console.Write(".");
                redis.GetSubscriber().Publish("outgoing", GenerateMessage());
                Thread.Sleep(500);
            }

        }

        static string GenerateMessage()
        {
            var rnd = new Random();
            var room = ROOMS[rnd.Next(0, ROOMS.Length)];
            var sensor = SENSORS[rnd.Next(0, SENSORS.Length)];
            var city = CITIES[rnd.Next(0, CITIES.Length)];
            var value = rnd.NextDouble() * 10.0;
            return $"{{city: '{city}', room: '{room}', sensor: '{sensor}', value: {value} }}";
        }
    }
}
