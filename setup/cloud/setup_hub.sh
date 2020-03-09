#!/bin/bash
subscriptionId=$1

resource_group_name="piday-hub-group"
iot_hub_name="piday-hub"

azc='az'

if ! [ -x "$(command -v az)" ]; then
  azc='az.cmd'
fi

echo "create $resource_group_name resource group"
$azc group create -l eastus -n $resource_group_name

echo "create $iot_hub_name hub"
$azc iot hub create --name $iot_hub_name --resource-group $resource_group_name --sku "S1" --location "eastus"
