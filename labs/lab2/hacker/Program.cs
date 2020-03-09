using System;
using System.Text.Json;
using System.Threading;
using Sense.RTIMU;

namespace lab2
{

    class Program
    {
        private static string RoomName = "living-room";
        // private static string RoomName = "dining-room";
        // private static string RoomName = "kitchen";
        // private static string RoomName = "basement";

        static void Main(string[] args)
        {
            var sensorReadingIntervalMilliseconds = 1000;

            using (var settings = RTIMUSettings.CreateDefault())
            {
                while (true)
                {
                    // SENSOR LOGIC HERE

                    Thread.Sleep(sensorReadingIntervalMilliseconds);
                }
            }
        }

        private static float CelsiusToFarenheight(float celsius)
        {
            return (celsius * (9 / 5)) + 32f;
        }

    }
}
