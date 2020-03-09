using System;
using System.Text.Json;
using System.Threading;
using Sense.RTIMU;
using Sense.Stick;

namespace lab2
{
    public class SensorReading<T>
    {
        public string city = "London";
        public string room { get; set; }
        public string sensor { get; set; }
        public T value { get; set; }
    }

    class Program
    {
        private static bool IsGarageDoorOpen = false;
        private static string RoomName = "living-room";
        // private static string RoomName = "dining-room";
        // private static string RoomName = "kitchen";
        // private static string RoomName = "basement";

        static void Main(string[] args)
        {
            var sensorReadingIntervalMilliseconds = 1000;

            var joystickEvents = Joystick.Events
                .Subscribe(p =>
                {
                    if (p.Key == JoystickKey.Enter && p.State == JoystickKeyState.Release)
                    {
                        IsGarageDoorOpen = !IsGarageDoorOpen;
                        if (IsGarageDoorOpen)
                        {
                            Console.WriteLine("Garage door is open");
                        }
                        else
                        {
                            Console.WriteLine("Garage door is closed");
                        }
                    }
                });

            using (var settings = RTIMUSettings.CreateDefault())
            {
                while (true)
                {
                    var temperature = GetTemperatureReading(settings);
                    var humidity = GetHumidityReading(settings);
                    var pressure = GetPressureReading(settings);
                    var garageDoor = GetGarageDoorReading();

                    Console.WriteLine(JsonSerializer.Serialize(temperature));
                    Console.WriteLine(JsonSerializer.Serialize(humidity));
                    Console.WriteLine(JsonSerializer.Serialize(pressure));
                    Console.WriteLine(JsonSerializer.Serialize(garageDoor));

                    Thread.Sleep(sensorReadingIntervalMilliseconds);
                }
            }
        }

        private static float CelsiusToFarenheight(float celsius)
        {
            return (celsius * (9 / 5)) + 32f;
        }

        private static SensorReading<bool> GetGarageDoorReading()
        {
            return new SensorReading<bool>
            {
                room = RoomName,
                sensor = "garage-door",
                value = IsGarageDoorOpen
            };
        }

        private static SensorReading<float> GetPressureReading(RTIMUSettings settings)
        {
            using (var pressureSensor = settings.CreatePressure())
            {
                try
                {
                    var reading = pressureSensor.Read();
                    return new SensorReading<float>
                    {
                        room = RoomName,
                        sensor = "pressure",
                        value = reading.Pressure
                    };

                }
                catch (System.Exception e)
                {
                    Console.WriteLine("Error reading from pressure sensor");
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        private static SensorReading<float> GetHumidityReading(RTIMUSettings settings)
        {
            using (var humiditySensor = settings.CreateHumidity())
            {
                try
                {
                    var reading = humiditySensor.Read();
                    return new SensorReading<float>
                    {
                        room = RoomName,
                        sensor = "humidity",
                        value = reading.Humidity
                    };

                }
                catch (System.Exception e)
                {
                    Console.WriteLine("Error reading from humidity sensor");
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        private static SensorReading<float> GetTemperatureReading(RTIMUSettings settings)
        {
            using (var humiditySensor = settings.CreateHumidity())
            {
                try
                {
                    var reading = humiditySensor.Read();
                    return new SensorReading<float>
                    {
                        room = RoomName,
                        sensor = "temperature",
                        value = CelsiusToFarenheight(reading.Temperatur)
                    };
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("Error reading from temperature sensor");
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }
    }
}
