# LAB 2 - READING SENSORS

### Prerequisite - Lab 0
If you haven't already gone through Lab 0 you might want to head back and check it out.

### Prerequisite - Lab 1
While not required, Lab 2 assumes you have followed lab 1 and already know how to view the code and connect to the Pi.

## TASK 1: Choose a room and a sensor
Following the Home Automation theme, each team will choose a _room_ and a _sensor_.

#### Rooms
|Room|Value|
|-----|-----|
|Living Room|```living-room```|
|Dining Room|```dining-room```|
|Kitchen|```kitchen```|
|Basement|```basement```|

#### Sensors
|Sensor|Snippet|
|-----|-----|
|Humidity|```settings.CreateHumidity()```|
|Pressure|```settings.CreatePressure()```|
|Temperature|```settings.CreateHumidity()``` _(this is not a typo)_|

Note the values and snippets above.

## TASK 2: Review the code
The _hacker_ folder contains some boilerplate code to get you started.  Using your preferred method from Lab1, open the Program.cs file in the lab2/hacker folder.

#### Get access to the IMU (Intertial Measurement Unit)
This library (part of SenseHatNet) will connect to a variety of multi-sensor chips, including the SenseHat

```var settings = RTIMUSettings.CreateDefault()```

It'll be wrapped in a C# using() block to ensure it is cleaned up when done.

#### Loop and wait
You only want to measure the sensors every second or so ... but you want to do it FOREVER!
```
var sensorReadingIntervalMilliseconds = 1000;

...

    while (true)
    {
        // SENSOR LOGIC HERE

        Thread.Sleep(sensorReadingIntervalMilliseconds);
    }

...
```

## TASK 3: Pick a room, create the sensor
Now that you've captured the IMU object (called settings above), you will need to get access to the sensor you wish to read.

First, Open Program.cs on your laptop or on the Pi directly (see Lab 1).

Next, update or uncomment the variable noting your room:
```
private static string RoomName = "living-room";
// private static string RoomName = "dining-room";
// private static string RoomName = "kitchen";
// private static string RoomName = "basement";
```

Finally, referring to the table above in TASK 1, add a using block to access your chosen sensor below the comment:

 ```// SENSOR LOGIC HERE```

 It should look something like this:
 ```
...

using (var settings = RTIMUSettings.CreateDefault())
{
    while (true)
    {
        // SENSOR LOGIC HERE
        using(var sensor = settings.CreatePressure()) 
        {
        
        }

        Thread.Sleep(sensorReadingIntervalMilliseconds);
    }
}

...
```
Of course, replace the "CreatePressure()" call with whichever sensor you are going to read.

## TASK 4: Read the sensor and test
Each sensor object has a different property used to retrieve the data:
|Sensor|Reading the data|
|-----|-----|
|Pressure|```sensor.Read().Pressure```|
|Humidity|```sensor.Read().Humidity```|
|Temperature|```sensor.Read().Temperatur``` _(also not a typo)_|

Now capture the value and toss it on the console output.  The code should look something like this:
```
using (var settings = RTIMUSettings.CreateDefault())
{
    while (true)
    {
        // SENSOR LOGIC HERE
        using(var sensor = settings.CreatePressure()) 
        {
            var data = sensor.Read().Pressure;
            Console.WriteLine(data);
        }

        Thread.Sleep(sensorReadingIntervalMilliseconds);
    }
}
```

### Run it
Now run this on the Pi and test! As a refresher...

#### From your laptop
| Step | Command |
|-----|-----|
|1. Build the app| ```dotnet build```|
|2. Publish it with a runtime|```dotnet publish -r linux-arm -o out```|
|3. Copy it to the Pi|```scp -r out pi@<IP_ADDRESS>:/home/pi/piday/labs/lab2/hacker/```|
|4. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|5. Get into the folder|```cd ~/piday/labs/lab2/hacker/out```|
|6. Make it executable|```chmod 777 lab2```|
|9. Run it!|```./lab2```|
|10. Kill it|Hit Ctrl-C to stop|

#### Directly on the Pi
| Step | Command |
|-----|-----|
|1. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|2. Get into the folder|```cd ~/piday/labs/lab2/hacker```|
|3. Make your edits| ```micro Programs.cs```|
|4. Run it|```dotnet run```|
|5. Kill it|Hit Ctrl-C to stop|

### Test it!
Now carefully try and get the values to change.  Breathe on the Pi to change the temperature or humidity. CAREFULLY raise and lower the Pi to watch the Pressure change.

## TASK 5: Format the output
You will use this lab later, get the sensor data into a JSON message that will be used as telemetry later. Add a simple string format to output JSON instead of a flat value.

The JSON format should be:


```{city: '', room: '', sensor: '', value: ''}```

Your code should look something like this:
```
...
using(var sensor = settings.CreatePressure()) 
{
    var data = sensor.Read().Pressure;
    var message = $"{{city: 'London', room: '{RoomName}', sensor: 'pressure', value: '{data}'}}";
    Console.WriteLine(message);
}
...
```

Of course, replace 'city' and 'sensor' with the correct information.

Your should see output like this:
```
{city: 'London', room: 'living-room', sensor: 'pressure', value: '970.24585' }
{city: 'London', room: 'living-room', sensor: 'pressure', value: '970.27246' }
{city: 'London', room: 'living-room', sensor: 'pressure', value: '970.2693' }
{city: 'London', room: 'living-room', sensor: 'pressure', value: '970.2893' }
{city: 'London', room: 'living-room', sensor: 'pressure', value: '970.2793' }
{city: 'London', room: 'living-room', sensor: 'pressure', value: '970.3064' }
```

## BONUS: Containerize!
Building on what you learned in Lab 1, put Lab 2 in a container image as well.

#### Build and publish
| Step | Command |
|-----|-----|
|1. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|2. Get into the folder|```cd ~/piday/labs/lab2/hacker```|
|3. Publish the output|```dotnet publish -r linux-arm -o out```|

NOTE the added parameter ```-r linux-arm``` which will ensure the native libraries included with SenseHatNet get bundled.

#### Create a Dockerfile
Use micro, nano, or vim to create a Dockerfile in the lab1/hacker director with the following contents:
```
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
COPY ./out/libRTIMULib* /usr/lib/
COPY ./out app/

ENTRYPOINT ["app/lab2"]
```

The additional COPY statement moves the native libraries into the correct place in the Docker image.  The Entrypoint has changed as well to directly execute the lab2 program.

#### Build Docker Image (and tag it as lab2)
```docker build . -t lab2```

#### Run the image (as privileged)
```docker run --rm --privileged lab2```

To stop and remove the image, just hit *Ctrl-C*.
