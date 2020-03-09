# LAB 3 - RGB MATRIX
In this lab, you will learn about the RGB matrix feature of the SenseHat. The goal will be to present a sensor reading as a color on the matrix.  The matrix will be split into quadrants to represent up to four sensor readings.

### Prerequisite - Lab 0
If you haven't already gone through Lab 0 you might want to head back and check it out.

### Prerequisite - Lab 1
While not required, Lab 3 assumes you have followed lab 1 and already know how to view the code and connect to the Pi.

## TASK 1: Light up some pixels
First, learn some of the basics of how the RGB Matrix lights up those pixels.

#### Open the hacker0 folder
Use what you learned in the previous labs to open the Program.cs file. There's not much there.

#### The Sense.Led Namespace
You already used this namespace in a previous lab, but there are several other classes:

|Class|Purpose|
|-----|-----|
|Sense.Led.LedMatrix|Controls the RGB matrix itself|
|Sense.Led.Pixels|Contains the list of pixels to display|
|Sense.Led.Cell|Specifies the location (row/col) of a pixel|
|Sensle.Led.CellColor|Specifies the location AND color of a pixel|

The key method calls are on the LedMatrix class:

|Method|Purpose|
|-----|-----|
|ShowMessage(text)|Scrolls the given text across the matrix|
|SetPixels(Pixels)|Displays the given pixels on the screen|
|SetLowLight(boolean)|Adjusts the gamma of the matrix and lowers the matrix's intensity|

#### The immutable list
When creating a list of Pixels to display, this library uses an _immutable_ list meaning that any addition to the list result in a completely new list.

The following creates a list of Pixels in the first row and the first three columns.

```
var pixelList = ImmutableList<CellColor>.Empty
				    .Add(new CellColor(new Cell(0, 0), new Color(255, 0, 0)))
				    .Add(new CellColor(new Cell(0, 1), new Color(0, 255, 0)))
				    .Add(new CellColor(new Cell(0, 2), new Color(0, 0, 255)));
```

#### Displaying the pixels
Once the list is created, sending them to the matrix is easy:

```
var pixels = new Sense.Led.Pixels(pixelList);
Sense.Led.LedMatrix.SetPixels(pixels);
Sense.Led.LedMatrix.SetLowLight(true);
```

#### Try it!
Update the Program.cs with the above lines of code. 

##### From your laptop
| Step | Command |
|-----|-----|
|1. Build the app| ```dotnet build```|
|2. Publish it with a runtime|```dotnet publish -r linux-arm -o out```|
|3. Copy it to the Pi|```scp -r out pi@<IP_ADDRESS>:/home/pi/piday/labs/lab3/hacker0/```|
|4. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|5. Get into the folder|```cd ~/piday/labs/lab3/hacker0/out```|
|6. Make it executable|```chmod 777 lab3```|
|9. Run it!|```./lab3```|
|10. Kill it|Hit Ctrl-C to stop|

##### Directly on the Pi
| Step | Command |
|-----|-----|
|1. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|2. Get into the folder|```cd ~/piday/labs/lab3/hacker0```|
|3. Make your edits| ```micro Programs.cs```|
|4. Run it|```dotnet run```|
|5. Kill it|Hit Ctrl-C to stop|

#### Have some fun
Experiment with some of the following:

```
Sense.Led.LedMatrix.SetPixels(pixels.Shift(1,1));
```

```
Sense.Led.LedMatrix.ShowMessage("Welcome!");
```

## TASK 2: Review refactored solution
Building from the lab 2, Lab 3's hacker solution begins with a more refactored code base to hide some of the basics.

#### Open the hacker1 folder
Use what you learned in the previous labs to open the Program.cs file.

#### Review refactorings
Look at the included methods which read each sensor:

```
var temperature = GetTemperatureReading(settings);
var humidity = GetHumidityReading(settings);
var pressure = GetPressureReading(settings);
```

These refactored methods simplify and clean up the code.  Take a look at the implementations in the Program.cs file.

Each of these methods return a SensorReading object which more easily create the desired message format (JSON):

```
public class SensorReading<T>
{
    public string city {get;set;}
    public string room {get;set;}
    public string sensor {get;set;}
    public T value {get;set;}
}
```

Finally, the effort in creating the pixel list and assigning each sensor to quadrant has been put into AddPixelsForReading.  Take a look at it and then find Steve and thank him for taking one for the team.

## TASK 3: Test the refactored code
By uncommenting some lines of code, you should be able to get the hacker1 solution back to where you left lab2.

#### Set your city and room
First thing is to update the static at the beginning to reflect your city here:

```
 private static string CITY = "London";

```

Next, uncomment the room you used in the previous lab, here:

```
// private static string RoomName = "living-room";
// private static string RoomName = "dining-room";
// private static string RoomName = "kitchen";
// private static string RoomName = "basement";
```

Uncomment the line which will read your chosen sensor:

```
// var temperature = GetTemperatureReading(settings);
// var humidity = GetHumidityReading(settings);
// var pressure = GetPressureReading(settings);
// var garageDoor = GetGarageDoorReading();

```

And write out the sensor reading to the console matching your sensor:

```
Console.WriteLine(JsonSerializer.Serialize(temperature));
```

Build, publish, copy, run and confirm the solution is working.

## TASK 4: Write your sensor value to the RGB matrix
Getting your individual sensor reading is just the start as the RGB matrix will include four different readings.

#### Combine multiple readings
The HomeAutomationReadings class has been created for you to capture readings from multiple sensors. For now, add your reading to the already created ```readings``` object by uncommenting the appropriate line in Program.cs:


```
// readings.TemperatureReading.Value = temperature.value;
// readings.HumidityReading.Value = humidity.value;
// readings.PressureReading.Value = pressure.value;
// readings.GarageDoorReading.Value = garageDoor.value;
```

#### Add the readings to the RGB matrix's pixel list
First, create a new List of "pixels" :


```
var pixelList = new List<CellColor>();
```

Then add your sensor's pixels to the list, replacing the appropriate parameters with your sensor reading:

```
AddPixelsForReading(pixelList, readings.TemperatureReading.Quadrant, readings.TemperatureReading.GetColor());
```

Finally, send those pixels to the Pi:

```
var immutablePixels = new Sense.Led.Pixels(ImmutableList.Create(pixelList.ToArray()));

Sense.Led.LedMatrix.SetPixels(immutablePixels);
Sense.Led.LedMatrix.SetLowLight(true);
```
*NOTE*: all the above work should be done inside the while loop and before the Thread.Sleep().

Your version should be similar to:

```
while (true)
{
          var temperature = GetTemperatureReading(settings);
          readings.TemperatureReading.Value = temperature.value;
          Console.WriteLine(JsonSerializer.Serialize(temperature));

          var pixelList = new List<CellColor>();
          AddPixelsForReading(pixelList, readings.TemperatureReading.Quadrant, readings.TemperatureReading.GetColor());

          var immutablePixels = new Sense.Led.Pixels(ImmutableList.Create(pixelList.ToArray()));
          Sense.Led.LedMatrix.SetPixels(immutablePixels);
          Sense.Led.LedMatrix.SetLowLight(true);

          Thread.Sleep(sensorReadingIntervalMilliseconds);
}


```


#### Test it!
Using methods you should have now mastered, get the code on to the Pi and run it!  Just in case you forgot...

##### From your laptop
| Step | Command |
|-----|-----|
|1. Build the app| ```dotnet build```|
|2. Publish it with a runtime|```dotnet publish -r linux-arm -o out```|
|3. Copy it to the Pi|```scp -r out pi@<IP_ADDRESS>:/home/pi/piday/labs/lab3/hacker1/```|
|4. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|5. Get into the folder|```cd ~/piday/labs/lab3/hacker1/out```|
|6. Make it executable|```chmod 777 lab3```|
|9. Run it!|```./lab3```|
|10. Kill it|Hit Ctrl-C to stop|

##### Directly on the Pi
| Step | Command |
|-----|-----|
|1. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|2. Get into the folder|```cd ~/piday/labs/lab3/hacker1```|
|3. Make your edits| ```micro Programs.cs```|
|4. Run it|```dotnet run```|
|5. Kill it|Hit Ctrl-C to stop|


#### Change the thresholds
In the same folder as Program.cs there are three classes: PressureReading.cs, HumidityReading.cs, TemperatureReading.cs.  

Each of these classes sets the color of the RGB pixels based on a thread hold value. Test the application with different thresholds.

## BONUS: Open and close the garage door
Simulate a garage door remote using the joystick/button on the SenseHat.

### TASK 1: Trap the button press
Just before your using statement, add the following code:

```
...
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
...

```

Unlike the other sensors, you use discrete events to handle the joystick button.

### TASK 2: Add Reading
Add this to your list of readings (inside the while loop):

```
var garageDoor = GetGarageDoorReading();
readings.GarageDoorReading.Value = garageDoor.value;
Console.WriteLine(JsonSerializer.Serialize(garageDoor));

```
### TASK 3: Add the pixels
And finally, add it to the pixel list:

```
AddPixelsForReading(pixelList, readings.GarageDoorReading.Quadrant, readings.GarageDoorReading.GetColor());

```

### TASK 4: Run it!
