# TellMeMyIP

This application will run forever, getting your local IP address and displaying it on the Raspberry Pi Sense Hat LED Matrix.

## Build the application for the Pi

	dotnet publish -r linux-arm -o dist

## Copy files to the pi

	scp -r dist pi@<IP_ADDRESS>:/home/pi/TellMeMyIp

## Make sure it is executable on the Pi

	chmod 777 ./TellMeMyIp