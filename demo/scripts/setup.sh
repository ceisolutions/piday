#!/bin/sh

subscription_id=$1
resource_group_name=$2
demo_name=$3
dps_primary_key=$4
dps_secondary_key=$5
dps_name="cei-${demo_name}-dps"
iot_hub_name="cei-${demo_name}-iothub"
acr_name="cei${demo_name}acr"

# login
az.cmd login

# set default subscription
echo "selecting subscription with id ${subscription_id}"
az.cmd account set --subscription $subscription_id
echo " subscription with id ${subscription_id} selected"

# deploy iot hub
echo "creating iot hub named ${iot_hub_name}"
az.cmd iot hub create --name $iot_hub_name --resource-group $resource_group_name --sku "S1" --location "eastus2"
echo "${iot_hub_name} created successfully"

# deploy dps
echo "creating device provisioning service named ${dps_name}"
az.cmd iot dps create --name $dps_name --resource-group $resource_group_name --location "eastus"
echo "${dps_name} created successfully"

# link dps with iot hub
echo "linking iot hub to device provisioning service"
hub_connection_string=$(az.cmd iot hub show-connection-string --name $iot_hub_name --key primary --query connectionString -o tsv)
echo $hub_connection_string
az.cmd iot dps linked-hub create --dps-name $dps_name --resource-group $resource_group_name --connection-string $hub_connection_string --location "eastus"
echo "iot hub successfully linked"

# create test group enrollment
echo "creating test enrollment group"
az.cmd iot dps enrollment-group create -g $resource_group_name --dps-name $dps_name --enrollment-id "test-enrollment-group" --ih "${iot_hub_name}.azure-devices.net" --ap "hashed" --rp "reprovisionandmigratedata" --primary-key $dps_primary_key --secondary-key $dps_secondary_key --tags "{'environment':'test'}"
echo "test enrollment group created successfully"

echo $'\e[1;33m'Manual Step '->' change ''IoT Edge Device'' setting to True in the portal and click ENTER$'\e[0m'
read -p "Press any key to continue... " -n1 -s

# create azure container registry
echo "creating azure container registry"
az.cmd acr create --resource-group $resource_group_name --name $acr_name --sku "Basic"
echo "azure container registry created successfully"

# get the dps scope id
scopeId="$(az.cmd iot dps show --name $dps_name -g $resource_group_name --query 'properties.idScope' -o tsv)"
echo "retrieved scope id $scopeId"

# create vms
echo "begin provisioning iot edge vms"
az.cmd vm image terms accept --urn "microsoft_iot_edge:iot_edge_vm_ubuntu:ubuntu_1604_edgeruntimeonly:latest"

echo "update deviceSetup.sh with dps primary key"
sed -i "3 s@.*@primaryKey=$dps_primary_key@g" deviceSetup.sh

counter=1
while [ $counter -le 2 ]
do
  vmName="test-edge-device-$counter"

  echo "creating test-edge-device-$counter"
  az.cmd vm create --resource-group $resource_group_name --name $vmName --image "microsoft_iot_edge:iot_edge_vm_ubuntu:ubuntu_1604_edgeruntimeonly:latest" --admin-username "azureuser" --generate-ssh-keys
  echo "created test-edge-device-$counter"

  echo "wait 180 seconds for vm"
  sleep 180s

  echo "run device setup on the vm"
  az.cmd vm run-command invoke --command-id RunShellScript -g $resource_group_name -n $vmName --script @deviceSetup.sh --parameters $scopeId $vmName
  
  ((counter++))
done

echo "creating test-edge-device-$counter"
az.cmd vm create --resource-group $resource_group_name --name "test-auto-provision-1" --image "microsoft_iot_edge:iot_edge_vm_ubuntu:ubuntu_1604_edgeruntimeonly:latest" --admin-username "azureuser" --generate-ssh-keys
echo "created test-edge-device-$counter"

echo "creating demo-edge-device-1"
az.cmd vm create --resource-group $resource_group_name --name "demo-edge-device-1" --image "microsoft_iot_edge:iot_edge_vm_ubuntu:ubuntu_1604_edgeruntimeonly:latest" --admin-username "azureuser" --generate-ssh-keys
echo "created demo-edge-device-1"


