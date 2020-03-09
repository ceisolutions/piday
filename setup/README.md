# SETUP FOR PI DAY
Most of the setup work will need to be done on the Raspberry Pi's themselves but there are requirements for networking as well as the instructors' laptops and cloud environment.

## RASPBERRY PI SETUP
The Pi's will run the Lite edition of Raspbian and will be setup to run disconnected from the Internet requiring that all prerequisites be downloaded, installed, configured and otherwise cached.

### Requirements
These labs are based on the Raspberry Pi 3 and have been tested on the B and the B+. They will most likely also with newer Pi's and most likely NOT work with earlier version.

Requirements:

* Raspberry Pi 3 B/B+
* SenseHat
* 8 GB+ microSD Card + Adapter and Reader

A keyboard and monitor will also be required for the initial setup.

### Base Image
The initial step will create a brand new image of Raspbian with connectivity to a WiFi network with Internet access.

#### 1. Download / Copy Image
The following instructions are to be done on a laptop/desktop.

Download Image:

* Download Raspbian Buster Lite from [the official source](https://www.raspberrypi.org/downloads/raspbian/)
* NOOBs is not required or suggested for this setup

Copy Image to SD Card:

* Download an image tool such as balenaEtcher (recommended for Windows)
* Follow instructions to copy the Raspbian image to the SD Card

#### 2. Setup Initial Networking

Mount the boot partition:

* Re-mount the SD card to ensure the BOOT partition is available
* A drive/partition named boot should be accessible


Setup initial WiFi network:

* Create a file named wpa_supplicant.conf with the content below
* Be sure to replace the information in the file with that of your WiFi network

wpa_supplicant.conf

```
ctrl_interface=/var/run/wpa_supplicant
update_config=1
country=us


network={
	ssid="PiNet"
	psk="piday2020"
}
```

** NOTE: This connection information will be replaced as part of the setup process.

Allow SSH access by default:

* In the same boot partition/drive
* Create a completely empty text file named _ssh_
* The file should have no extension


### 3. Initial Device Setup
The remainder of the setup will be done ON the Pi and is almost entirely automated.

#### Install the SenseHat
You can figure it out.  JUST DON'T ATTACH OR DETACH WITH THE PI POWERED ON. Please.

#### Get connected
Use an SSH client of your choice (see lab0) and get connected to the Pi OR use a directly attached keyboard.

|Step|Command|
|-----|------|
|1. Connect Pi|Connect the Pi to a monitor (and a keyboard if needed)|
|2. Get address|Once the pi has finished booting, the IP address should be visible in the initial log stream|
|3. Connect with SSH|Optionally, connect from your favorite ssh client|
|4. Login|User: pi, Password: raspberry|

#### Update the apt repositories
Once logged into the Pi, update the installer repositories:

```sudo apt-get update```

#### Clone this repository
Git will be used to clone this repository

|Step|Command|
|-----|-----|
|1. Install Git|```sudo apt-get install git```|
|2. Clone|```git clone <TBD> /home/pi/piday```

#### Run the setup script
This script will do the following:

* Install Docker / Docker-Compose
* Install Vim and micro (editors)
* Download Docker base layers (.NET SDK/Runtime)
* Install .NET SDKs (2.1 and 3.1)
* Pre-cache NuGet packages
* Install SenseHat shared libraries
* Pre-build IP and Messaging Docker solutions
* Set IP solution to launch at startup / stop and login
* Enable I2C
* Reset networking
* Reboot

**NOTE**: If you plan on using a different WiFi network instead of the private WLAN, FIRST modify the wpa_supplicant.conf file in the ~/piday/setup/pi/scripts folder. 

Run the script:

|Step|Command|
|-----|-----|
|1. CD into the script folder|```cd ~/piday/setup/pi/```|
|2. Run as root|```sudo ./setup.sh```|

Wait. Patiently.

#### Confirmation
Once the Pi reboots, the device should attempt to connect to the PiNet network.  If successful, it will marquee the IP address across the RGB matrix of the SenseHat.

#### Disconnect Power / Remove SD Card
FIRST, disconnect the power from the Pi and then remove the SD card for cloning.

### 4. Device (SD Card) cloning (Windows)
You can optionally run the setup procedure on each and every pi. On at a time. For hours on end. Instead, consider just cloning the SD card over and over.

Download and install [Win32 Disk Imager](https://sourceforge.net/projects/win32diskimager/)

*NOTE*: If you have different size SD cards, create an image from the _smallest_ card. Larger to smaller will not work.

Create an image file:

|Step|Description|
|-----|-----|
|1. Specify Image|In the Image file field, select a location where the image will be saved|
|2. Mount Card|Using an SD card reader/adapter, insert an already setup Pi microSD Card|
|3. Select drive|Select the first drive that is on the card (usually boot), all others will be imaged as well|
|4. Save Image|Click Read|

Clone an image file:

|Step|Description|
|-----|-----|
|1. Mount Card|Using an SD card reader/adapter, insert the desired card|
|2. Erase Card||
|3. Specify Image|In the Image file field, select the previously saved image|
|4. Select drive|Select the first drive that is on the card|
|5. Write Image|Click Write|


## NETWORKING SETUP
These labs are setup to run completely disconnected and on a private WiFi network, not routed to the Internet. This creates a known state but everything will most likely work with the Pi's connected to a public network.

### 1. Router Requirements
Go to your basement and get that router that you still have hoping someday it will be used to run a fun lab of disconnected Raspberry Pi's.

OR, buy the cheapest WiFi router that has an Ethernet switch as you will need connect the instructor laptop to it.

### 2. Router Setup
The Pi's have been setup to connect to a wireless network with the following connection details:

|Property|Value|
|-----|-----|
|SSID|PiNet|
|PSK/Password|piday2020|
|Subnet|192.168.2.nnn|

If you would like to use different network, please ensure you have already modified the wpa_supplicant.conf file as instructed above.

Default router admin account. For consistency, please consider using the following completely insecure and now public information:

|Property|Value|
|-----|------|
|Username|admin|
|Password|piday2020admin|

If you are an attendee reading this, we are at your mercy. Please consider messing with the router only AFTER everyone has already completed their labs. With any luck, the instructor hasn't completely ignored these instructions and setup an admin password different from the above.

### 3. Instructor laptop connectivity
You might have a laptop that can connect to two different wireless networks.  Good luck. For the rest of us, please ensure you have the appropriate adapters and an Ethernet cable and connect directly to the router.

Also, ensure you are connected to a wireless network with a route to the Internet.

*NOTE*: Windows Firewall likes to learn to block the sender and listener console apps. If they mysteriously stop working, check to see if they are blocked.

### 4. Pi Connectivity
Using an already configure SD card, boot the Raspberry Pi and wait, patiently, for it to complete booting. Eventually, the Pi's IP address will marquee across the RGB matrix display. It will update when/if the network changes.  

Login to the Pi. If successful, the user startup script will stop the IP address container and stop displaying.

## CLOUD SETUP
For the cloud lab, attendees will use pre-created and shared Azure AD accounts and resource groups.

### 1. Create accounts and resource groups for each city
The cloud/setup.sh script can be used for creating a set of users and resource groups for a given city.  If you don't already have a locally signed in Azure CLI running bash, consider using the online Azure Shell (with Bash), clone this repo and then run the script as follows:

```setup.sh <subscription> <city_name> <count> <password>```

You will likely need to allow execute permissions:

```chmod 777 setup.sh```

As an example:

```setup.sh 123123-123123-123123123-123123 London 3 Password123!```

This will create the following:

* In the subscrption with the ID: 123123-123123-123123123-123123 ...
* Three resource groups named: piday-London-team-1, piday-London-team-2, piday-London-team-3
* Three Azure AD accounts named: Londonpidayuiser1@piday.dev, Londonpidayuiser2@piday.dev, Londonpidayuiser3@piday.dev
* Sets each account's password to: Password123!
* Grants contributor rights for each account to the similarly named resource group

*Don't forget to delete these when PiDay is done*

### 2. Deploy shared IoT Hub Solution
All cities will use the same hub for fun times. The cloud/setup_hub.sh script will create the following:

* Resource group named: piday-hub-group
* IoT Hub (Standard): piday-hub

Same instructions as above, just passing in the sub ID:

```setup_hub.sh 12341-1231234-1231234-123124```

Again, you might need to add exec permissions:

```chmod 777 setup_hub.sh```

### 2. Power BI
TBD

### 3. IoT Hub Demos
See demos/...

### 4. Create a device for each site/city
Once the shared IoT hub has been created, create a device to use for your given city.

|Step|Instruction|
|-----|-----|
|Open Portal|Access the Hub created above via the Azure Portal|
|Open Devices|Click IoT Devices in the left panel|
|Add Device|Click the + button to add a new device|
|Name it|Best bet is <city>-piday-relay as in london-piday-relay|
|Create|Leave everything else as is, click Save|

Once the device is done creating, you will need the connection string to add into the Relay solution on your laptop:

|Step|Instruction|
|-----|-----|
|Select Device|Click refresh on the devices page and click on your device|
|Copy Connection|Grab a copy of the Primary Connection String, keep it handy|
|Update Relay.cs|In the messaging/Relay/Program.cs update the appropriate string|

Do not push this back to GitHub.

## INSTRUCTOR LAPTOP SETUP
The instructor laptop will be used for demos, presenting content, searching the Internet, collaborating with other cities over Teams and finally it will act as a relay/gateway from the telemetry broadcast from the Pi's to the IoT Hub.

### 1. Requirements
Some of this is obvious, some is not...

* Completed lab0 to ensure you have the same base setup as the attendees.
* Cloned copy of this repository
* Ethernet port or adapter
* microSD card reader/adapter
* VS Code extensions (Azure IoT)
* A Pi of your own
* An Azure account
* Power BI Desktop (pro license for streaming)

### 2. Pre-build Relay solution
For the cloud demos and labs, the instructor laptop will relay messages broadcast from the Pi's and sends them to the IoT hub.

|Step|Instructions|
|-----|-----|
|1. Get IoT Hub connection string|Create a new IoT Device in VSCode's Azure IoT Extension and obtain the connection string in the IoT Hub for your city|
|2. Update connection|In the messaging/relay/Program.cs file, update the connection string|
|3. DO NOT| Do not commit this to GitHub, please|
|4. Build Docker solution|In the messaging folder, ```docker-compose build...```|

*NOTE* For Windows, you cannot have the listener in host networking mode. You will need to run the Listener separately. Unless you are on Docker in WSL2, that is.

Windows prep:

|Step|Command|
|-----|-----|
|1. Build the listener (messaging/listener)|```dotnet build```|
|2. Build the relay (messaging/relay|```dotnet build```|

REVIEW the instructions in the messaging/README.md file.

### 3. Connect to the PiNet router
To ensure the relay will work, connect your laptop to a wireless router (Ethernet) and power on a Pi. Ensure you can SSH into the Pi.

### 4. Follow lab prep...
Just follow lab0 and lab1 to ensure you all the same tools, connectivity, etc. as the attendees will.
