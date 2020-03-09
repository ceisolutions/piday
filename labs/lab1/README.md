# LAB 1 - WELCOME TO PI DAY
In this lab you'll get connected to your Raspberry Pi and run your first .NET Core app ... ON the Pi! We'll shake out any connection issues before moving onto the next lab.

### Prerequisite - Lab 0
If you haven't already gone through Lab 0 you might want to head back and check it out.

## TASK 1: Connect to the Raspberry Pi
To communitate with the Pi, you will need to connect to the PiNet Wifi (NO INTERNET!) and use your SSH tool to connect.

| Step | Command |
|-----|-----|
|1. Connect to the PiNet WiFi network| SSID: PiNet, PWD: piday2020|
|2. Determine your Pi's IP address | Check the RGB matrix on the Pi |
|3. Open an SSH connection |  ssh pi@192.168.2.nnn |
|4. Enter the default password | raspberry |

## TASK 2: Review the Lab folders
All of the labs will have two folders: _hacker_ and _slacker_.

#### Hacker Folders
These folders contain a _starter_ application with the boring stuff already in place for you. But that's it, the rest is on you.

#### Slacker Folders
These folders contain a _completed_ application in the event you get stuck, run into time issues, or just don't want to type out that code. Some of the labs will start with a previous lab's _slacker_ folder.


## TASK 3: Review the application
The labs will assume you are using Visual Studio Code but you should be able to subsitute any text editor. The below provide steps for both using Visual Studio Code _and_ for using an editor __directly__ on the Pi.

### Using Visual Studio Code
Open the lab1/hacker folder in VS Code on your laptop.

### Editing on the Pi
Feeling like a hacker today? Or, corporate IT has you locked down? Worry not, the command line is in your future!

| Step | Command |
|-----|-----|
|1. Open an SSH connection |  ssh pi@192.168.2.nnn |
|2. Get into the lab1 folder | cd ~/piday/labs/lab1/hacker |
|3. Open Program.cs |  micro Program.cs |
|4. Exit when done | Ctrl-Q |

Nano and Vim are also installed on the Pi should you prefer a different editor.

### The Code
The labs use the amazing [SenseHatNet](https://www.nuget.org/packages/SenseHatNet/) library to make interacting with the SenseHat a breeze.

##### lab1.csproj
Open lab1.csproj and find some key items...

Target .NET Core 3.1:
```<TargetFramework>netcoreapp3.1</TargetFramework>```

Remove warnings about packaging:
```<NoWarn>NU1605</NoWarn>```

Bring in a reference to SenstHatNet:
```<PackageReference Include="SenseHatNet" Version="0.0.5" />```

#### Program.cs
Open Program.cs and review the following...

```Sense.Led.LedMatrix.ShowMessage("Welcome to pi day!");```

Hmmm, wonder what that does.

## TASK 4: Run it!
For this and all the other labs, you'll need to build, package and deploy the program to the (or on the) Pi and then finally run it. Choose one path (or BOTH paths) below.

### On your laptop
You'll use the ```dotnet``` command line to build and publish the application then copy it to the Pi. Since you probably don't have an ARM architecture CPU you will need to tell the dotnet CLI to target it.

| Step | Command |
|-----|-----|
|1. Open a command prompt in the lab1/hacker folder|Try VS Code's built in terminal!|
|2. Build the app| ```dotnet build```|
|3. Publish it with a runtime|```dotnet publish -r linux-arm -o out```|
|4. Copy it to the Pi|```scp -r out pi@<IP_ADDRESS>:/home/pi/piday/labs/lab1/hacker/```|
|5. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|6. Get into the folder|```cd ~/piday/labs/lab1/hacker/out```|
|7. Make it executable|```chmod 777 lab1```|
|8. Run it!|```./lab1```|


### Directly on the Pi
Skip all that GUI nonsense and hack right on the Pi!
| Step | Command |
|-----|-----|
|1. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|2. Get into the folder|```cd ~/piday/labs/lab1/hacker```|
|3. Run it|```dotnet run```|

You can optionally publish the output executable:

```dotnet publish -o out```

## BONUS: Containerize!
Need a small challenge? Wondering how one might deploy this program? Try this...

#### Build and publish
| Step | Command |
|-----|-----|
|1. Connect to the Pi| ```ssh pi@<IP_ADDRESS>``` |
|2. Get into the folder|```cd ~/piday/labs/lab1/hacker```|
|3. Publish the output|```dotnet publish -o out```|

#### Create a Dockerfile
Use micro, nano, or vim to create a Dockerfile in the lab1/hacker director with the following contents:
```
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
COPY ./out app/

ENTRYPOINT ["dotnet", "app/lab1.dll"]
```
#### Build Docker Image (and tag it as lab1)
```docker build . -t lab1```

#### Run the image (as privileged) once (and remove it)
```docker run --rm --privileged lab1```

#### What?
The Dockerfile started with a .NET Core 3.1 runtime (the FROM statement) copied the previously published files to the "app" folder in the image and set it to run when the container did.

The docker run command instructs the docker runtime to run in "priviledged" mode allowing access to the Sense Hat hardware.  The --rm switch makes sure to remove the running instance when it is done.

If you run into any issues, take a look in the slacker folder.