#!/bin/bash

subscriptionId=$1
city=$2
teamCount=$3
password=$4

azc='az'

if ! [ -x "$(command -v az)" ]; then
  azc='az.cmd'
fi


# login
$azc login

$azc account set --subscription $subscriptionId
echo "subscription $subscription selected"

counter=1

while [ $counter -le $teamCount ]
do
  resource_group_name="piday-$city-team-$counter"
  user_principal_name="${city}pidayuser${counter}@piday.dev"
  user_name="${city}pidayuser${counter}"

  echo "create $resource_group_name resource group"
  $azc group create -l eastus -n $resource_group_name

  echo "create $user_principal_name user"
  $azc ad user create --display-name $user_name \
                        --password $password \
                        --user-principal-name $user_principal_name \
                        --force-change-password-next-login false
  
  sleep 1m

  echo "create contributor role assignment"
  $azc role assignment create --role 'Contributor' \
                                --assignee $user_principal_name \
                                --resource-group $resource_group_name
  sleep 1m
  
  $azc role assignment create --role 'Reader' \
                                --assignee $user_principal_name \
                                --resource-group 'piday-hub-group'
  sleep 1m

  echo "create cosmos ${user_name}_db in $resource_group_name"
  $azc cosmosdb create \
	  --name "${user_name}db" \
	  --resource-group $resource_group_name \
	  --locations regionName=eastus
  
  sleep 1m
	  
  az cosmosdb sql database create \
  	--name "pi-day" \
	--account-name "${user_name}db" \
	--resource-group $resource_group_name
	
  sleep 1m

  az cosmosdb sql container create \
  	--account-name "${user_name}db" \
	--database-name "pi-day" \
	--name "sensor-data" \
	--partition-key-path "/_id" \
	--resource-group $resource_group_name
  
  ((counter++))
done


