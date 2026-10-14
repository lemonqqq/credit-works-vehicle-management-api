# CreditWorks Vehicle Management API

Backend for the CreditWorks Software Engineer technical assignment. The intended application manages vehicles and weight categories using ASP.NET Core and SQL Server.

> **Project status:** This README describes the target behaviour and design decisions. The API now registers an EF Core SQL Server DbContext, defines model-managed seed data, and implements vehicle create/update with application-level identifier checks. Vehicle reads and category endpoints remain stubs. There is no migration or completed web interface yet.

## Assignment scope

Users must be able to add vehicles; view and sort all vehicles; see each vehicle's current weight category and icon; and create, edit, and delete categories. Category definitions must cover every valid vehicle weight without gaps or overlaps. Changes to categories must immediately affect the category shown for existing vehicles. Data must persist in SQL Server 2019 or later.

## Vehicle

| Field | Requirement | Notes |
| --- | --- | --- |
| `id` | Required, server generated | GUID. |
| `ownerName` | Required | Nonblank string. |
| `manufacturerId` | Required | References a predefined manufacturer. |
| `year` | Required | Sensible year of manufacture. |
| `weightKg` | Required | Positive decimal in kilograms, with at most two decimal places. |
| `vin` | Required **project extension** | Unique after trimming and case normalization. The assignment does not require this field. |
| `registrationNumber` | Required **project extension** | At most six characters; unique after trimming and case normalization. The assignment does not require this field. |
| `description` | Optional **project extension** | Free text. |

The core assignment requires only owner name, manufacturer, year, and weight. Requiring VIN and registration number is an additional product decision and makes the create contract stricter than the brief; revisit this decision if the reviewer expects vehicles to be created using only the four assignment fields.

`Make` is treated as another name for manufacturer, rather than stored twice. Model, WOF expiry, service expiry, and status are out of scope for this exercise. The proposed vehicle update operation keeps weight immutable after registration; category changes can still change its calculated category.

### Manufacturers

Seed Mazda, Mercedes, Honda, Ferrari, and Toyota. Store manufacturers as data referenced by vehicles, rather than repeating an enum or string list throughout the code. A read endpoint supplies the vehicle form. Adding or modifying manufacturers is a possible extension; the assignment does not require manufacturer administration.

### Vehicle list

Return owner name, manufacturer, year, and the **current** category name and icon for every vehicle. Weight must remain available to the backend for categorisation and sorting, even if the UI does not display it.

Support sorting by `ownerName`, `manufacturer`, `year`, and `weightKg`. Ascending and descending order are supported as a project choice. Return the applied sort field and direction, and use `id` as a stable tie-breaker. Pagination is an additional choice; apply sorting and pagination in the database and return the total count. Optional filters may include manufacturer, year, and weight range. The previously proposed make/model/status sorts and make/model filters do not apply because those are not separate fields in this design.

## Vehicle categories

| Field | Requirement | Notes |
| --- | --- | --- |
| `id` | Required, server generated | GUID. |
| `name` | Required | Nonblank; unique among categories. |
| `description` | Optional | Free text. |
| `minWeightKg` | Required | Inclusive lower boundary; nonnegative decimal with two decimal places. |
| `maxWeightKgExclusive` | Optional | Exclusive upper boundary; `null` means no upper limit. |
| `iconKey` | Required | Key for a bundled, allowlisted icon asset. |

Valid vehicle weights are greater than zero. Category ranges use **inclusive lower and exclusive upper** boundaries: `[minWeightKg, maxWeightKgExclusive)`. The first category starts at `0`; adjacent boundaries must be equal; exactly one final category has no upper boundary. Each finite upper boundary must be greater than its lower boundary. This covers all positive weights without gaps or overlaps.

Seed the following configuration:

| Category | Range | Boundary examples |
| --- | --- | --- |
| Light | `0 < weightKg < 500.00` | `499.99` is Light. |
| Medium | `500.00 <= weightKg < 2500.00` | `500.00` is Medium. |
| Heavy | `weightKg >= 2500.00` | `2500.00` is Heavy. |

Assign an icon to each seeded category. Icon keys resolve to application assets; do not accept arbitrary file paths or executable content as icon values.

### Category changes

The category returned for a vehicle is calculated from its weight and the **current** ranges. Do not persist a vehicle-to-category relationship that can become stale. A change to ranges must therefore be reflected in subsequent vehicle reads without rewriting vehicle records.

Validate a proposed category configuration as a whole before saving it. Reject gaps, overlaps, invalid bounds, missing icons, duplicate names, or a missing open-ended final range. Use a transaction and concurrency protection so concurrent edits cannot leave an invalid configuration. Single-category create, edit, and delete operations must also validate their resulting full configuration. A configuration preview can report errors and show how existing vehicles would be categorised; applying the configuration remains a separate explicit action. Moving a shared boundary between two categories should use the atomic configuration endpoint.

## HTTP contracts (planned)

These describe the target contracts. Vehicle create/update currently implement only part of their described behaviour; vehicle reads and category endpoints are still stubs. JSON property names below describe the intended API model. Use consistent validation responses (for example, Problem Details), `404` for missing records, and sanitized `5xx` responses for unexpected failures. Do not expose stack traces or connection strings.

| Method and route | Behaviour |
| --- | --- |
| `GET /health` | Return `200 OK` when the host is running. This is an extra operational endpoint, not an assignment requirement. |
| `POST /api/vehicles` | Validate required fields, manufacturer reference, sensible year, positive two-decimal weight, and the project-specific VIN/registration rules. Persist the vehicle; return `201 Created` with ID and calculated current category/icon. |
| `GET /api/vehicles/{id}` | Return a vehicle and its current calculated category/icon, or `404 Not Found`. Supports the optional edit flow. |
| `PUT /api/vehicles/{id}` | Replace editable details of an existing vehicle, enforcing the same validation and uniqueness rules. Reject weight changes under the immutable-weight assumption; return the updated vehicle or `404`. Vehicle editing is an extra feature beyond the brief. |
| `GET /api/vehicles/list` | Return the vehicle list with owner, manufacturer, year, current category/icon, total count, pagination, and applied sort. Accept `sortBy` values `ownerName`, `manufacturer`, `year`, or `weightKg`; `direction=asc` or `desc`; `page`; and `pageSize`. Optionally accept manufacturer/year/weight filters. |
| `GET /api/categories` | Return categories ordered by minimum weight, including ranges and icon keys. |
| `POST /api/categories` | Create a category only if the resulting complete range configuration remains valid; return `201 Created` or a clear validation error. |
| `PUT /api/categories/{id}` | Edit a category, validate the complete resulting configuration, and return the updated category; return `404` for an unknown ID. |
| `DELETE /api/categories/{id}` | Delete a category only if coverage remains complete and unambiguous; otherwise return a clear validation error. |
| `POST /api/categories/configuration/preview` | Validate a proposed complete category set without saving it. Return gaps/overlaps/boundary errors and a summary of how existing vehicles would be classified. This is an optional aid to the review-before-apply flow. |
| `PUT /api/categories/configuration` | Validate and atomically replace the proposed complete category configuration. Reject invalid or conflicting changes; subsequent vehicle reads use the new categories immediately. |

The category administration routes must not allow a sequence of individually valid requests to leave an invalid state. The configuration endpoint is the preferred way to split a range, merge ranges, or move a boundary shared by two categories.

## Validation and data integrity

- Validate on the server; frontend validation is only an additional usability measure.
- Reject blank owner and category names, unknown manufacturers, missing icons, invalid years, nonpositive vehicle weights, and weights with more than two decimal places.
- Use precise decimal storage (for example, SQL `decimal(18,2)`) for weights and range boundaries; do not use floating-point types.
- Check VIN and registration uniqueness in the application before vehicle creation or update, after trimming and case normalization. The database does not have unique indexes for these fields. Without a database constraint, concurrent requests or writes outside this API can still create duplicates.
- Make category validation and persistence atomic. Ensure each positive weight resolves to exactly one category at all times.
- Handle malformed input, absent records, invalid category edits/deletions, uniqueness conflicts, and database failures with clear API errors.

## Technical approach

- **API:** ASP.NET Core controllers handle HTTP requests, responses, and input validation.
- **Application/domain:** Services enforce category coverage, boundary rules, vehicle validation, and category calculation independently of controllers.
- **Data:** Entity Framework Core with SQL Server 2019 or later is the intended persistence approach. Use migrations and seed the five manufacturers and three initial categories. Keep connection strings in local configuration or user secrets, never in source control.
- **Category calculation:** Resolve categories from current ranges at read time. No vehicle-category foreign key is required.
- **Security:** Use parameterised EF Core queries, validate user input, restrict icon keys to known assets, and return safe error messages. Authentication and authorisation are not required by the assignment.

The initial scaffold targets .NET 10. Keep the architecture proportionate to this small application; the existing project folders do not imply that every feature is implemented or wired into the solution yet.

## Automated tests to add

- Vehicle validation, including missing core fields, invalid years, nonpositive weights, decimal precision, and project-specific VIN/registration uniqueness.
- Category determination immediately below, at, and above `500.00` and `2500.00` kg.
- Rejection of range gaps, overlaps, invalid bounds, and deletion of a required range.
- Reclassification of existing vehicles after an atomic category change.
- Vehicle sorting by each required field and both directions; pagination stability if pagination is retained.
- Integration tests for SQL persistence, seeding, transactions, and concurrent category edits where practical.

## Getting started

### Prerequisites

- .NET 10 SDK, matching the current project target.
- SQL Server 2019 or later (SQL Server Express is acceptable) for the completed persistence implementation.

### Current implementation

From the repository root, the existing API project can be built and run with:

1. Initialize database - run below powershell in the project folder
```powershell
# add EntityFramework packages
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# initialize DB - create database and run migration script to create table and seeded data
Set-Location credit-works-vehicle-management-api\build
.\initiailize-db.ps1 -ServerInstance "(localdb)\MSSQLLocalDB"
```

The solution includes the API, application, domain, infrastructure, data, and test projects. Four unit tests cover application-level VIN and registration checks. Vehicle create/update can use SQL Server after a database has been created; the remaining vehicle read and category actions are not implemented.

### Database setup (pending implementation)

The EF Core SQL Server provider, DbContext registration, and model-managed seed definitions are present. Configure `ConnectionStrings:DefaultConnection` through user secrets or an environment variable; the committed `Database` settings are empty. An initial migration and documented migration-application command are still needed. A reviewer must be able to clone the repository, configure SQL Server, apply migrations, build, run, and test without reverse-engineering the schema or performing undocumented manual steps.

## Scope and known limitations

- The assignment requires a web interface for vehicle listing and category administration. Frontend implementation is outside this API repository's current scaffold.
- Vehicle change auditing, authentication, and authorisation are out of scope for the assessment. A production deployment would revisit these choices.
- VIN, registration number, description, vehicle editing, filters, pagination, preview, and health checking are project choices beyond the assignment's minimum requirements.
- SQL Server model configuration and seed definitions exist, but no migration has been created or applied. Vehicle read, category administration, category calculation, and other business-rule tests remain to be implemented.
- SQL and data encryption is out of scope for this assessment as it is not required.
