# API tests

Run the .NET unit tests from this repository:

```powershell
dotnet test CreditWorks.VehicleManagement.Unit.Test/CreditWorks.VehicleManagement.Unit.Test.csproj
```

The suite covers command and aggregate validation, complete category configurations, category maintenance, vehicle service reads and writes, cache behavior, and the EF Core seed/model contract. The EF model tests inspect metadata and do not connect to SQL Server. The current vehicle contract still requires VIN and registration number; the unit tests describe that behavior, while the assessment requires a vehicle to be creatable with only owner, manufacturer, year, and weight.

If MSBuild fails without diagnostics while resolving project references on this machine, run with `-m:1 -p:MSBuildEnableWorkloadResolver=false`. The test host may also need to run outside a restricted sandbox.

SQL-backed endpoint scenarios live in the sibling app repository's `tests/e2e/api.spec.js`. See its `TESTING.md` for disposable database setup, mutation safeguards, and the requirement coverage map.
