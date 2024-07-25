// main.bicep

param location string = resourceGroup().location
param appName string = 'countryapi-adora'

// Resource group variables
var appServicePlanName = '${appName}-plan'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'F1' // Free tier for Windows plan
    tier: 'Free'
    size: 'F1'
    family: 'F'
    capacity: 1
  }
  kind: 'app'
  properties: {
    reserved: false // Windows Plan
  }
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
  }
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
}

output appServiceUrl string = webApp.properties.defaultHostName
