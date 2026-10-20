# CreditWorks Vehicle Management API

ASP.NET Core 10 API with Entity Framework Core and SQL Server for the vehicle management assessment. The sibling [React app](../credit-works-vehicle-management-app/README.md) provides the UI. Requirements below describe the intended behavior; the status notes identify what the current code actually delivers.

## Run locally

Prerequisites: .NET 10 SDK, SQL Server 2019 or later (LocalDB is suitable locally), and the `dotnet-ef` tool. From this directory:

```powershell
cd .\credit-works-vehicle-management-api
.\build\initialize-db.ps1 -ServerInstance "(localdb)\MSSQLLocalDB"
dotnet build .\CreditWorks.VehicleManagement.slnx
dotnet run --project .\CreditWorks.VehicleManagement\CreditWorks.VehicleManagement.csproj --launch-profile http
```

The script stores `ConnectionStrings:DefaultConnection` in API user secrets and applies the checked-in `InitialCreate` migration. It defaults to database `CreditWorksDB`; `-DatabaseName` overrides this. Alternatively, set `ConnectionStrings:DefaultConnection` or all of `Database:Server`, `Database:Name`, `Database:Username`, and `Database:Password`. The latter path uses SQL encryption and requires a trusted server certificate. The development HTTP profile listens on `http://localhost:5155`; Swagger UI is at `/swagger` in Development. `GET /health` reports host availability, not database health. Migrations are not run automatically at startup.

Run `dotnet test CreditWorks.VehicleManagement.Unit.Test/CreditWorks.VehicleManagement.Unit.Test.csproj` for .NET tests. See [TESTING.md](TESTING.md) and the app's [TESTING.md](../credit-works-vehicle-management-app/TESTING.md) for mocked UI and SQL-backed scenarios.

## Functional requirements

| Requirement | Current API behavior |
| --- | --- |
| Create a vehicle | `POST /api/vehicles` accepts owner name, manufacturer name, year, VIN, registration number, and weight in kg. A GUID is generated when queued work runs. VIN and registration are required project extensions beyond the assessment's four minimum fields. |
| Read and update vehicles | `GET /api/vehicles/{id}` returns vehicle details or `404`. `PUT /api/vehicles/{id}` queues replacement of an existing record; weight can change. No vehicle delete endpoint is exposed. |
| List, search, sort, and page vehicles | `GET /api/vehicles/search` returns `{ data, pageNumber, totalRecords }`. Filters are owner-name substring, manufacturer-name substring, exact year, and inclusive minimum/maximum weight. The store takes 10 rows per page. `sortFieldList` can specify fields and directions for Dynamic LINQ; the UI currently sorts one field on its loaded page instead of sending this parameter. |
| Use predefined manufacturers | SQL seed data contains Mazda, Mercedes, Honda, Ferrari, and Toyota. Vehicle writes look up a manufacturer by name; unknown names fail when queued work executes. There is no manufacturer list or administration endpoint. |
| Manage weight categories | `GET /api/categories/list` returns category definitions. `PATCH /api/categories` accepts a **complete** `{ "categories": [...] }` set: omit/`null` ID for a new category, retain an existing ID to edit, and omit an existing category to delete. The service validates the full set, then applies it in a transaction. There are no individual category mutation or preview endpoints. |
| Maintain one current category per positive vehicle weight | Every valid configuration begins at 0, has adjacent nonoverlapping lower-inclusive/upper-exclusive ranges, and ends with one open upper bound. Vehicles store weight but no category ID. The API returns vehicle and category data separately; the UI calculates the current category and icon from the latest ranges. |

Initial data includes 20 vehicles and Light `[0, 500)` (`car`), Medium `[500, 2500)` (`van`), and Heavy `[2500, infinity)` (`truck`). Thus 500.00 kg is Medium and 2500.00 kg is Heavy. Category names must be nonblank and unique ignoring case within a proposed set; icon keys must be nonblank; boundaries have at most two decimal places. A finite upper bound must exceed its lower bound. Multiple categories may reuse an icon key.

Vehicle validation requires nonblank owner, a known manufacturer, year from 2000 through the next UTC calendar year (the command also caps it at 2050), a 17-character alphanumeric VIN, a 1–6-character alphanumeric registration, and positive weight with at most two decimal places. Owner and identifiers are trimmed on save. The service checks VIN and registration duplicates on create/update. The SQL model uses `decimal(18,2)` for weight and boundaries.

### HTTP contract

| Method and path | Response |
| --- | --- |
| `GET /health` | `200` with `{ "status": "healthy" }`. |
| `GET /api/vehicles/{id}` | Vehicle DTO or `404`. |
| `GET /api/vehicles/search` | `{ data, pageNumber, totalRecords }`. Query fields: `ownerName`, `manufacturerName`, `year`, `minWeightKg`, `maxWeightKg`, `pageNumber`, and `sortFieldList` items with `fieldName` and `isAscending`. |
| `POST /api/vehicles` | `202 Accepted` with a queue GUID when enqueued. |
| `PUT /api/vehicles/{id}` | `202 Accepted` with a queue GUID when enqueued. |
| `GET /api/categories/list` | Array of `{ id, name, description, minWeightKg, maxWeightKg, iconKey }`. |
| `PATCH /api/categories` | `202 Accepted` with a queue GUID for a complete configuration request. |

Handled validation returns Problem Details (`400`), missing records `404`, and business conflicts `409`; an unexpected controller error yields a generic `500`. A full write queue returns `503`. Some validation, not-found, and conflict checks run **after** a write has been accepted, so these statuses may not reach the original caller.

## Non-functional requirements and current limits

- **Persistence and consistency:** EF Core persists vehicles, manufacturers, and categories in SQL Server. Category replacement uses a transaction and clears the in-process category cache. The single queue reader serializes writes only within this API process; multiple instances or external database writers are not coordinated. Category reads are cached in memory without cross-instance invalidation.
- **Queued execution:** The bounded in-process channel holds up to 100 pending writes. One background worker processes them in fresh service scopes. This is an assessment demonstration of responsive request handling: callers can return to other work while the server processes a write. `GET /api/request/{id}` returns a queued, running, succeeded, or failed status; reading a terminal status removes it. Queue contents and statuses are lost on restart. `202 Accepted` means queued, not saved. Durable work storage is outside this assessment.
- **Validation and integrity:** Server-side command/domain checks and full category-set validation protect normal API writes. A follow-up migration adds unique VIN and registration indexes, which must be applied to an existing database to enforce uniqueness. Category icon keys are restricted to `bike`, `car`, `van`, and `truck`.
- **Security:** EF Core uses parameterized queries for ordinary filters. Sort fields are handled through a fixed set of expressions. No authentication/authorization is configured; local assessment use is assumed.
- **Observability and errors:** Responses receive an `X-Correlation-ID` header and requests are logged with it. Worker failures are logged and reported as `failed` in the status endpoint without an error detail. Development mode exposes OpenAPI/Swagger; unexpected controller errors return a generic response.
- **Paging and performance:** Search filters and returns a fixed 10-row page in SQL, then reports the total match count. `pageNumber` is the requested page number when matches exist, including when the requested page is empty; it is 0 when there are no matches. Sorting uses the vehicle ID as a stable tie-breaker. No measured performance target is defined.
- **Availability and deployment:** `/health` checks only that the host responds. The in-memory queue/cache design targets one local machine. A cloud deployment would need shared durable work storage, retries, status reporting, and cache coordination. Amazon SQS, Azure Queue Storage/Service Bus, or a database-backed queue are possible replacements; these were deliberately out of scope for the assessment.
- **Testing and accessibility:** Unit tests cover validation, services, cache, and EF metadata; SQL-backed endpoint tests require a disposable database. UI accessibility review/remediation is outside assessment scope; no compliance claim is made.

## Scope assumptions and possible enhancements

- Manufacturer add/update/delete and a database-managed icon catalog, including more icon options, are future features. The five manufacturers are seeded; the UI hardcodes manufacturer and icon options.
- VIN and registration were **added and implemented** for duplicate checks, although the assessment minimum names only owner, manufacturer, year, and weight. Database uniqueness constraints are a possible enhancement.
- The assessment assumes owner and manufacturer names contain only alphanumeric characters. The current code does not consistently enforce a literal rule, and seeded owner names contain spaces. A production rule should define treatment of spaces and punctuation.
- The design assumes no more than 10 categories, but the validator does not enforce a limit. The same icon may be reused by several categories.
- A public queue-status endpoint and completion notification, separate command/query hosts, authentication, and accessibility work are outside the current scope.
