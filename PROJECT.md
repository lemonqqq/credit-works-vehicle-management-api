# Project requirements and HTTP contract

## Objective and scope

The assessment asks for a maintainable web application that persists vehicles and editable weight categories in SQL Server. The [assessment brief](CreditWorks%20Software%20Engineer.pdf) is the source of requirements; this file records the chosen interpretation and current HTTP contract. Current gaps are tracked in [OUTSTANDING.md](OUTSTANDING.md).

## Functional requirements

| ID | Requirement | Design / current contract |
| --- | --- | --- |
| F1 | Create a vehicle with mandatory owner, predefined manufacturer, sensible year, and positive kilogram weight to two decimal places. | `POST /api/vehicles`; current implementation additionally requires VIN and registration number. |
| F2 | Persist and display vehicles with owner, manufacturer, year, and current category icon. | SQL-backed `GET /api/vehicles/search`; UI combines weight with current category definitions. |
| F3 | Sort by owner, manufacturer, year, and weight; show selected order. | API accepts sort fields and directions; UI offers all four fields and both directions but sorts the loaded page locally. |
| F4 | Seed Mazda, Mercedes, Honda, Ferrari, and Toyota as manufacturers. | EF Core seed data; no manufacturer list endpoint exists. |
| F5 | Seed Light, Medium, and Heavy with icons; assign exactly one category to every positive weight. | `[0,500)`, `[500,2500)`, `[2500,infinity)`; icon keys `car`, `van`, `truck`. |
| F6 | Create, edit, and delete categories, including name, range, and icon. | `PATCH /api/categories` submits a complete set: a missing ID creates, a retained ID updates, and omission deletes. The current UI flow needs completion. |
| F7 | Reject category gaps, overlaps, invalid boundaries, or loss of complete coverage. | Server validates the full set before transactional replacement. |
| F8 | Existing vehicles reflect changed category definitions. | Category is calculated from stored weight and current ranges, not persisted on the vehicle. The UI must reload after completed changes. |

## Non-functional requirements and design constraints

- **Data:** SQL Server 2019 or later with checked-in EF Core migration and seed data. Database initialization must be reproducible from a clone.
- **Maintainability:** Clear API/application/domain/infrastructure separation, readable code, and tests for consequential business rules.
- **Integrity:** Server-side validation, transactional category replacement, decimal precision, and safe handling of expected errors. `202 Accepted` writes require status polling to establish completion.
- **Security:** Keep secrets out of Git, use parameterized data access, and return safe error responses. Authentication is outside the assessment.
- **Deployment:** One machine and one API process with minimal setup. In-memory queue/cache are deliberate local trade-offs. No measured throughput or latency target is defined.
- **Usability:** Show validation errors and selected sorting clearly. Responsive layout exists, but no accessibility compliance claim is made.

## HTTP contract implemented by the API

JSON property names below use the default ASP.NET Core camel-case format. Base URL for the local HTTP profile: `http://localhost:5155`.

| Method and path | Request | Success |
| --- | --- | --- |
| `GET /health` | None | `200 {"status":"healthy"}`; host only. |
| `GET /api/vehicles/{id}` | GUID path parameter | `200` vehicle `{id, ownerName, manufacturer, year, vin, registrationNumber, weightKg}`. |
| `GET /api/vehicles/search` | Optional `ownerName`, `manufacturerName`, `year`, `minWeightKg`, `maxWeightKg`, `pageNumber`, and indexed `sortFieldList` (`fieldName`, `isAscending`). | `200 {data:[...], pageNumber, totalRecords}`; page size 10. Sort field names: `ownerName`, `manufacturer`, `year`, `weight`. |
| `POST /api/vehicles` | `{ownerName, manufacturer, year, vin, registrationNumber, weightKg}` | `202` with a queue GUID in the JSON body. |
| `PUT /api/vehicles/{id}` | Same vehicle body | `202` with a queue GUID. |
| `GET /api/categories/list` | None | `200` array of `{id, name, description, minWeightKg, maxWeightKg, iconKey}`. |
| `PATCH /api/categories` | `{categories:[{id?, name, description?, minWeightKg, maxWeightKg, iconKey}, ...]}` as the entire configuration | `202` with a queue GUID. |
| `GET /api/request/{id}` | Queue GUID | `200 {state}` with `queued`, `running`, `succeeded`, or `failed`; terminal read removes the status. |

Malformed or model-invalid input returns `400` Problem Details. Missing records can return `404`, business conflicts `409`, a full queue `503`, and an unexpected controller failure a generic `500`. For accepted writes, validation or database errors that occur in the worker surface as queue state `failed` rather than changing the original `202` response. A second poll after terminal status consumption returns `404`.

## Assumptions and out of scope

- Boundary wording in the brief is ambiguous at 500 and 2500 kg; this project uses lower-inclusive and upper-exclusive ranges.
- The current implementation requires VIN and registration in addition to the four assessment fields. This is an extension that currently blocks the minimum create contract.
- There are no individual category mutation endpoints, manufacturer administration endpoints, vehicle deletion, or persisted category assignment.
- Authentication, production hosting, distributed operation, durable queue storage, comprehensive accessibility certification, and load testing are outside this local assessment.
- Automatic repair of category gaps/overlaps, regional formats, and multiple UI languages are future enhancements in [ENHANCEMENT.md](ENHANCEMENT.md).
