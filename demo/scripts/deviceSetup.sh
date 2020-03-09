scope=$1
regId=$2
primaryKey=

set -e
configFile='/etc/iotedge/config.yaml'

keybytes=$(echo $primaryKey | base64 --decode | xxd -p -u -c 1000)
symmetricKey=$(echo -n $regId | openssl sha256 -mac HMAC -macopt hexkey:$keybytes -binary | base64)
  
#comment out the manual device provisioning configuration
sed -i '31 s@.*@#provisioning:@g' $configFile
sed -i '32 s@.*@#  source:@g' $configFile
sed -i '33 s@.*@#  device_connection_string@g' $configFile

sed -i '45 s@.*@provisioning:@g' $configFile
sed -i '46 s@.*@  source: "dps"@g' $configFile
sed -i '47 s@.*@  global_endpoint: "https:\/\/global.azure-devices-provisioning.net"@g' $configFile
sed -i '48 s@.*@  scope_id: '"\"$scope\""'@g' $configFile
sed -i '49 s@.*@  attestation:@g' $configFile
sed -i '50 s@.*@    method: "symmetric_key"@g' $configFile
sed -i '51 s@.*@    registration_id: '"\"$regId\""'@g' $configFile
sed -i '52 s@.*@    symmetric_key: '"\"$symmetricKey\""'@g' $configFile

sudo systemctl restart iotedge