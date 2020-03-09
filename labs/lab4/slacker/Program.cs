using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using Sense.Led;
using Sense.RTIMU;
using Sense.Stick;
using StackExchange.Redis;

namespace lab4
{
    class Program
    {
        private static bool IsGarageDoorOpen = false;
        private static string CITY = "London";
        private static string RoomName = "living-room";
        // private static string RoomName = "dining-room";
        // private static string RoomName = "kitchen";
        // private static string RoomName = "basement";
        private static HomeAutomationReadings readings;

        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            var sensorReadingIntervalMilliseconds = 1000;
            readings = new HomeAutomationReadings();

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


            redis
                .GetSubscriber()
                .Subscribe("incoming")
                .OnMessage(message => DisplaySensorReadings(message.Message));

            using (var settings = RTIMUSettings.CreateDefault())
            {
                while (true)
                {
                    var temperature = GetTemperatureReading(settings);
                    var humidity = GetHumidityReading(settings);
                    var pressure = GetPressureReading(settings);
                    var garageDoor = GetGarageDoorReading();

                    readings.TemperatureReading.Value = temperature.value;
                    readings.HumidityReading.Value = humidity.value;
                    readings.PressureReading.Value = pressure.value;
                    readings.GarageDoorReading.Value = garageDoor.value;

                    redis.GetSubscriber().Publish("outgoing", JsonSerializer.Serialize(temperature));
                    redis.GetSubscriber().Publish("outgoing", JsonSerializer.Serialize(humidity));
                    redis.GetSubscriber().Publish("outgoing", JsonSerializer.Serialize(pressure));
                    redis.GetSubscriber().Publish("outgoing", JsonSerializer.Serialize(garageDoor));

                    Thread.Sleep(sensorReadingIntervalMilliseconds);
                }
            }

        }

        private static void DisplaySensorReadings(RedisValue message)
        {
            var sensorType = JsonSerializer.Deserialize<SensorType>(message.ToString());

            if (sensorType.sensor == "temperature")
            {
                var reading = JsonSerializer.Deserialize<SensorReading<float>>(message.ToString());
                readings.TemperatureReading.Value = reading.value;
                Console.WriteLine($"Temperature {readings.TemperatureReading.Value}");
            }
            else if (sensorType.sensor == "humidity")
            {
                var reading = JsonSerializer.Deserialize<SensorReading<float>>(message.ToString());
                readings.HumidityReading.Value = reading.value;
                Console.WriteLine($"Humidity {readings.TemperatureReading.Value}");
            }
            else if (sensorType.sensor == "pressure")
            {
                var reading = JsonSerializer.Deserialize<SensorReading<float>>(message.ToString());
                readings.PressureReading.Value = reading.value;
                Console.WriteLine($"Pressure {readings.TemperatureReading.Value}");
            }
            else if (sensorType.sensor == "garage-door")
            {
                var reading = JsonSerializer.Deserialize<SensorReading<bool>>(message.ToString());
                readings.GarageDoorReading.Value = reading.value;
                Console.WriteLine($"Garage Door {readings.TemperatureReading.Value}");
            }
            else
            {
                Console.WriteLine($"Received unknown sensor: {sensorType.sensor}");
            }

            var pixelList = new List<CellColor>();

            AddPixelsForReading(pixelList, readings.TemperatureReading.Quadrant, readings.TemperatureReading.GetColor());
            AddPixelsForReading(pixelList, readings.HumidityReading.Quadrant, readings.HumidityReading.GetColor());
            AddPixelsForReading(pixelList, readings.PressureReading.Quadrant, readings.PressureReading.GetColor());
            AddPixelsForReading(pixelList, readings.GarageDoorReading.Quadrant, readings.GarageDoorReading.GetColor());

            var immutablePixels = new Sense.Led.Pixels(ImmutableList.Create(pixelList.ToArray()));

            Sense.Led.LedMatrix.SetPixels(immutablePixels);
            Sense.Led.LedMatrix.SetLowLight(true);
        }

        private static float CelsiusToFarenheight(float celsius)
        {
            return (celsius * (9 / 5)) + 32f;
        }

        private static SensorReading<bool> GetGarageDoorReading()
        {
            return new SensorReading<bool>
            {
                city = CITY,
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
                        city = CITY,
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
                        city = CITY,
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
                        city = CITY,
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

        static void AddPixelsForReading(List<CellColor> pixelList, Quadrant quadrant, Color color)
        {
            var blank = new Color(0, 0, 0);
            var rows = 8;
            var columns = 8;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (row < rows / 2)
                    {
                        // Top Half
                        if (column < columns / 2)
                        {
                            // Top Left Quadrant
                            if (quadrant == Quadrant.TopLeft)
                            {
                                pixelList.Add(new CellColor(new Cell(row, column), color));
                            }
                        }
                        else
                        {
                            // Top Right Quadrant
                            if (quadrant == Quadrant.TopRight)
                            {
                                pixelList.Add(new CellColor(new Cell(row, column), quadrant == Quadrant.TopRight ? color : blank));
                            }
                        }
                    }
                    else
                    {
                        // Bottom Half
                        if (column < columns / 2)
                        {
                            // Bottom Left Quadrant
                            if (quadrant == Quadrant.BottomLeft)
                            {
                                pixelList.Add(new CellColor(new Cell(row, column), quadrant == Quadrant.BottomLeft ? color : blank));
                            }
                        }
                        else
                        {
                            // Bottom Right Quadrant
                            if (quadrant == Quadrant.BottomRight)
                            {
                                pixelList.Add(new CellColor(new Cell(row, column), quadrant == Quadrant.BottomRight ? color : blank));
                            }
                        }
                    }
                }
            }
        }
    }
}
