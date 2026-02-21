param(
    [string]$Environment = "Development",
    [string]$KeyVaultUri = "",
    [string]$KeyVaultSecretName = "CosmosConnectionString",
    [string]$CosmosConnectionString = "",
    [string]$CosmosDatabaseId = "WeddingDatabase",
    [string]$CosmosContainerId = "Users",
    [switch]$NoRun
)

$ErrorActionPreference = "Stop"

$env:ASPNETCORE_ENVIRONMENT = $Environment
$env:KEYVAULT__VAULTURI = $KeyVaultUri
$env:KEYVAULT__COSMOSCONNECTIONSTRINGSECRETNAME = $KeyVaultSecretName
$env:COSMOS__DATABASEID = $CosmosDatabaseId
$env:COSMOS__CONTAINERID = $CosmosContainerId

if ([string]::IsNullOrWhiteSpace($CosmosConnectionString)) {
    Remove-Item Env:COSMOS__CONNECTIONSTRING -ErrorAction SilentlyContinue
}
else {
    $env:COSMOS__CONNECTIONSTRING = $CosmosConnectionString
}

Write-Host "Environment variables set for WeddingWebApp."
Write-Host "ASPNETCORE_ENVIRONMENT=$($env:ASPNETCORE_ENVIRONMENT)"
Write-Host "KEYVAULT__VAULTURI=$($env:KEYVAULT__VAULTURI)"
Write-Host "KEYVAULT__COSMOSCONNECTIONSTRINGSECRETNAME=$($env:KEYVAULT__COSMOSCONNECTIONSTRINGSECRETNAME)"
Write-Host "COSMOS__DATABASEID=$($env:COSMOS__DATABASEID)"
Write-Host "COSMOS__CONTAINERID=$($env:COSMOS__CONTAINERID)"

if ($NoRun) {
    Write-Host "NoRun specified. Skipping application startup."
    exit 0
}

Write-Host "Starting WeddingWebApp..."
dotnet run --project .\WeddingWebApp.csproj
