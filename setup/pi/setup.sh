#!/usr/bin/env bash
if ! [ -x "$(command -v docker)" ]; then
	echo "Installing Docker ..."
	curl -sSL https://get.docker.com | sh
	usermod -aG docker pi
fi

echo "Updating packages ..."
apt-get update 

echo "Installing tools ..."
apt-get install git -y
apt-get install vim -y
if ! [ -x "$(command -v micro)" ]; then
	curl https://getmic.ro | bash
	mv -f ./micro /usr/local/bin 
fi

apt-get install docker-compose -y

scriptdir=`dirname "$BASH_SOURCE"`

echo "Getting Docker Images ..."
$scriptdir/docker_images.sh

echo "Check for dotnet 2.1"
if ! [ -d "/home/pi/dotnet/sdk/2.1.804" ]; then
	echo "Downloading dotnet 2.1 ..."
	wget https://download.visualstudio.microsoft.com/download/pr/365bc2c9-eb08-4c17-b462-e4addba6dd0a/5101726ab1e41e751c169e3b735357fc/dotnet-sdk-2.1.804-linux-arm.tar.gz -O dotnet21.gz

	echo "Installing dotnet 2.1 ..."
	mkdir -p /home/pi/dotnet 
	chmod a+rwx /home/pi/dotnet
	tar zxf dotnet21.gz -C /home/pi/dotnet
fi

echo "Check for dotnet 3.1"
if ! [ -d "/home/pi/dotnet/sdk/3.1.101" ]; then
	echo "Downloading dotnet 3.1 ..."
	wget https://download.visualstudio.microsoft.com/download/pr/d52fa156-1555-41d5-a5eb-234305fbd470/173cddb039d613c8f007c9f74371f8bb/dotnet-sdk-3.1.101-linux-arm.tar.gz -O dotnet31.gz

	echo "Installing dotnet 3.1 ..."
	mkdir -p /home/pi/dotnet 
	chmod a+rwx /home/pi/dotnet
	tar zxf dotnet31.gz -C /home/pi/dotnet
fi

if ! [ -x "$(command -v dotnet)" ]; then
	echo "# DOTNET" >> /home/pi/.profile
	echo "PATH=$PATH:/home/pi/dotnet" >> /home/pi/.profile
	echo "DOTNET_ROOT=/home/pi/dotnet" >> /home/pi/.profile
	echo "export DOTNET_ROOT" >> /home/pi/.profile
fi

echo "Caching Nuget packages ..."
sudo -u pi /home/pi/dotnet/dotnet restore $scriptdir/lab0.csproj

echo "Copy sense hat libs to /usr/lib"
/bin/cp /home/pi/.nuget/packages/sensehatnet/0.0.5/runtimes/linux-arm/native/* /usr/lib


echo "Build Tell me my ip container for cache"
docker-compose -f $scriptdir/TellMeMyIp/docker-compose.yml build
echo "Build messaging containers for cache"
docker-compose -f $scriptdir/../../messaging/docker-compose.yml build

echo "Add tell me my ip to startup"
/bin/cp $scriptdir/scripts/stopIPScroll.sh /etc/profile.d
/bin/cp $scriptdir/scripts/myip.service /etc/systemd/system
chmod +x /etc/profile.d/stopIPScroll.sh 
systemctl enable myip.service

echo "Adding I2C support"
/bin/cp $scriptdir/scripts/i2c.conf /etc/modules-load.d
/bin/cp $scriptdir/scripts/config.txt /boot

echo "Setting KB to US"
/bin/cp -rf $scriptdir/scripts/keyboard /etc/default/

echo "Updating wireless"
/bin/cp -rf $scriptdir/scripts/wpa_supplicant.conf /etc/wpa_supplicant/

echo "Cleaning up ..."
rm -f *.gz

reboot
