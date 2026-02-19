param(
    [string]$Environment = "Development",
    [string]$KeyVaultUri = "",
    [string]$KeyVaultSecretName = "CosmosConnectionString",
    [string]$CosmosConnectionString = "",
    [string]$CosmosDatabaseId = "WeddingDatabase",
    [string]$CosmosContainerId = "Users",
    [string]$CosmosPartitionKeyPath = "/pk",
    [bool]$CosmosAutoCreateResources = $true,
    [switch]$NoRun
)

$ErrorActionPreference = "Stop"

$env:ASPNETCORE_ENVIRONMENT = $Environment
$env:KeyVault__VaultUri = $KeyVaultUri
$env:KeyVault__CosmosConnectionStringSecretName = $KeyVaultSecretName
$env:Cosmos__DatabaseId = $CosmosDatabaseId
$env:Cosmos__ContainerId = $CosmosContainerId
$env:Cosmos__PartitionKeyPath = $CosmosPartitionKeyPath
$env:Cosmos__AutoCreateResources = $CosmosAutoCreateResources.ToString()

if ([string]::IsNullOrWhiteSpace($CosmosConnectionString)) {
    Remove-Item Env:Cosmos__ConnectionString -ErrorAction SilentlyContinue
}
else {
    $env:Cosmos__ConnectionString = $CosmosConnectionString
}

Write-Host "Environment variables set for WeddingWebApp."
Write-Host "ASPNETCORE_ENVIRONMENT=$($env:ASPNETCORE_ENVIRONMENT)"
Write-Host "KeyVault__VaultUri=$($env:KeyVault__VaultUri)"
Write-Host "KeyVault__CosmosConnectionStringSecretName=$($env:KeyVault__CosmosConnectionStringSecretName)"
Write-Host "Cosmos__DatabaseId=$($env:Cosmos__DatabaseId)"
Write-Host "Cosmos__ContainerId=$($env:Cosmos__ContainerId)"
Write-Host "Cosmos__PartitionKeyPath=$($env:Cosmos__PartitionKeyPath)"
Write-Host "Cosmos__AutoCreateResources=$($env:Cosmos__AutoCreateResources)"

if ($NoRun) {
    Write-Host "NoRun specified. Skipping application startup."
    exit 0
}

Write-Host "Starting WeddingWebApp..."
dotnet run --project .\WeddingWebApp.csproj
