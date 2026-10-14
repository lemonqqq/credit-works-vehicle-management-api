param(
    [Parameter(Mandatory = $true)]
    [string]$ServerInstance,

    [string]$DatabaseName = "CreditWorksDB"
)

$ErrorActionPreference = "Stop"
$env:ASPNETCORE_ENVIRONMENT = "Development"

$apiProject = Join-Path $PSScriptRoot "../CreditWorks.VehicleManagement/CreditWorks.VehicleManagement.csproj"
$infraProject = Join-Path $PSScriptRoot "../CreditWorks.VehicleManagement.Infrastructure/CreditWorks.VehicleManagement.Infrastructure.csproj"

$connectionString = "Server=$ServerInstance;Database=$DatabaseName;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"

dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString --project $apiProject
if ($LASTEXITCODE -ne 0) { throw "Could not save the local connection string." }

dotnet ef database update InitialCreate --project $infraProject --startup-project $apiProject
if ($LASTEXITCODE -ne 0) { throw "Database migration failed. Check the SQL Server instance name, service, and permissions." }

Write-Host "Database is ready."