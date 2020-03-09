# LAB 4 - Messaging
In this lab, you will start sending your sensor data to a messaging bus and you will read sensor data from your city. The goal will be to send your sensor reading so that it can be received by you and others in your city. Then, you will also read sensor data you and others in your city send up and display that data on the LED Matrix.

### Prerequisite - Lab 3
Lab 4 is an extension of Lab 3. You learned how to use the LED matrix and you also have the sensor information. It's time to use those skills and share your sensor information with your city.

### Prerequisite - Messaging Setup
In order to share your sensor data with others, you'll need to start running the network messagaing solution. In the "messaging" folder, a Docker Compose file will launch a Redis Pub/Sub service, a network listener and a network receiver.  Take a look at the README in the Messaging folder if you are curious on how this works.

#### Launch the messaging solution

| Step | Command |
|-----|-----|
|1. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|2. Get into the folder|```cd ~/piday/messaging```|
|3. Run in the backgroud|```docker-compose up -d```|
|4. Verify|```docker ps```|

Running ```docker ps``` should show three containers running: redis, listener, sender.  If you see all three, you are ready to move on.

## TASK 1: Send your sensor data to the messaging bus
Instead of simply console logging the sensor data, it's time to share that data with the rest of the attendees. To do that you're going to use a Redis pub/sub channel to broadcast our readings. To do so you'll want to make sure you're bringing in the correct using statements.  

Add these to the top of Program.cs:

```
using System;
using StackExchange.Redis;
```

Then you need to establish a connection to redis. Initialize this as one of the first things you do in the program.

Add this just inside your main() loop:

```
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
```

*NOTE*: That you are connecting to your local Redis service.  This was launched in a Docker container as part of the setup you did above.

You already have a nicely formed json string that you've been logging out, now it's time to send that json string out for everyone to see. Change your Console.WriteLine (just before the Thread.Sleep) of the Serialized sensor reading to:

```
redis.GetSubscriber().Publish("outgoing", JsonSerializer.Serialize(temperature));
```

That's it for sending your sensor information up! Now everyone can process that information as they see fit.

## TASK 2: Receive sensor data from the room
Now that you are sending sensor data out to the room, you can start reading data in. Remember that you'll be reading in your own data too. 

Receiving these messages and displaying to the RGB matrix no longer happens in the while loop. You'll be responding to events instead of displaying our own sensors right away. Let's start with a little refactor. Move all of the code for displaying the sensor data on the RGB Matrix into it's own method. You'll end up with something like this (just after the Main method block):

```
private static void DisplaySensorReadings()
{
    var pixelList = new List<CellColor>();

    AddPixelsForReading(pixelList, readings.TemperatureReading.Quadrant, readings.TemperatureReading.GetColor());

    var immutablePixels = new Sense.Led.Pixels(ImmutableList.Create(pixelList.ToArray()));

    Sense.Led.LedMatrix.SetPixels(immutablePixels);
    Sense.Led.LedMatrix.SetLowLight(true);
}
```

A change that you may notice is that the variable _readings_ is used but not available in this scope. If you recall, this variable is for storing the current values for all sensor readings. This variable now needs to be elevated to a class level field.

Add the following just _above_ the main() method block:

```
private static HomeAutomationReadings readings;
```

This can then be initialized in the Main method where it was previously created.

```
readings = new HomeAutomationReadings();
```

Now that you have a new method for displaying the sensor data you can read that sensor data. You'll want to place this code before the while loop. It is event-based so you'll just be told when you have new sensor data available and it will call in to refresh our RGB Matrix with the new values. You'll start by writing this code:

```
redis
    .GetSubscriber()
    .Subscribe("incoming")
    .OnMessage(message => DisplaySensorReadings(message.Message));
```

You're subscribing to the _incoming_ channel on the Redis pub/sub messaging bus and when you get a new message you're calling into your new method with that message. The problem is you don't handle that message coming into our new method. That `message.Message` is of type `RedisValue`. You need to parse that back into a SensorReading<T> object but, because that message is a generic type, that's a bit more difficult than it should be. You are going to make that type casting a bit easier to see at the cost of more lines of code.

In order to show the new sensor value in the RGB Matrix, you need to find out what type of sensor value you received and update the current sensor reading object with that value in the correct SensorReading<T> object. To do this, the `SensorType` class has been provided for you to use. This sensor type class simply has a `sensor` property on it so you can deserialize the message string into something you can check the type of sensor you received.

Once you know the type of sensor you received, you can cast the message into the correct SensorReading<float> or SensorReading<bool> object and update the latest reading. Here is how the `DisplaySensorReadings` method has changed:

```
private static void DisplaySensorReadings(RedisValue message)
{
    var sensorType = JsonSerializer.Deserialize<SensorType>(message.ToString());

    if (sensorType.sensor == "temperature")
    {
        var reading = JsonSerializer.Deserialize<SensorReading<float>>(message.ToString());
        readings.TemperatureReading.Value = reading.value;
        Console.WriteLine($"Temperature {readings.TemperatureReading.Value}");
    }
    else
    {
        Console.WriteLine($"Received unknown sensor: {sensorType.sensor}");
    }

    var pixelList = new List<CellColor>();

    AddPixelsForReading(pixelList, readings.TemperatureReading.Quadrant, readings.TemperatureReading.GetColor());

    var immutablePixels = new Sense.Led.Pixels(ImmutableList.Create(pixelList.ToArray()));

    Sense.Led.LedMatrix.SetPixels(immutablePixels);
    Sense.Led.LedMatrix.SetLowLight(true);
}
```

## Test it!
Using methods you should have now mastered, get the code on to the Pi and run it!  Just in case you forgot...

### From your laptop
| Step | Command |
|-----|-----|
|1. Build the app| ```dotnet build```|
|2. Publish it with a runtime|```dotnet publish -r linux-arm -o out```|
|3. Copy it to the Pi|```scp -r out pi@<IP_ADDRESS>:/home/pi/piday/labs/lab4/hacker/```|
|4. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|5. Get into the folder|```cd ~/piday/labs/lab4/hacker/out```|
|6. Make it executable|```chmod 777 lab4```|
|9. Run it!|```./lab4```|
|10. Kill it|Hit Ctrl-C to stop|

### Directly on the Pi
| Step | Command |
|-----|-----|
|1. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|2. Get into the folder|```cd ~/piday/labs/lab4/hacker```|
|3. Make your edits| ```micro Programs.cs```|
|4. Run it|```dotnet run```|
|5. Kill it|Hit Ctrl-C to stop|


Leave it running! That data will be used next!

## BONUS: 
Try out filtering the sensor data you read in. Maybe you only want to display certain rooms or even certain values. Explore different options of displaying the sensor data on the LED matrix. You could try averaging the values as they come in to create a smooth color change. You could also try dedicating one LED to each room in the quadrant. 
