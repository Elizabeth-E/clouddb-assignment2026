# Architecture Overview

This project is a local, containerized system with clear separation between data services, business logic, and API/trigger layers. It is designed so each data service can scale independently. Steps for setup can be found in the readme

## Deployment Environment
- Local development is defined in `docker-compose.yml`.
- Services are run in containers: SQL Server, Azurite (Blob/Queue/Table), SQLPad, and Azure Functions.
- The Web API runs locally (outside Docker by default) but can be containerized separately if needed.

## Tiers
- **Data Tier**
  - SQL Server for relational data (applications, offers, houses, applicants).
  - Blob storage (Azurite) for offer documents and house photos.
- **Business Tier**
  - Web API services in `BuyMyHouseApi/Services` (validation, rules, and data access orchestration).
  - Azure Functions in `Functions/Services` (batch processing and offer generation).
- **Presentation Tier**
  - Not required for this project. API + Swagger serve as the interaction layer.

## Scalability & Resource Allocation
- Each data source has a dedicated service/container:
  - `sql` (SQL Server) for relational data.
  - `azurite` for blob/queue/table storage.
  - `functions` for background/batch processing.
- Data services can be scaled independently by moving to managed equivalents:
  - SQL Server -> Azure SQL Database
  - Azurite -> Azure Storage Accounts
  - Functions -> Azure Functions (Consumption/Premium)

## Capacity & Throughput Considerations
- Blob storage uses hierarchical blob paths for natural partitioning:
  - Offer documents: `offers/{applicant-name}/{applicationId}/{offerId}.json`
- SQL tables use indexed foreign keys for access patterns:
  - Examples: `MortgageApplications.ApplicantId`, `MortgageOffers.ApplicationId`.
- For cloud deployment, the intended scale points are:
  - SQL: scale up/down the service tier based on workload.
  - Blob: storage account throughput scales automatically; blob paths avoid hot partitions.
- This local setup mirrors production services so throughput tuning can be applied without major code changes.

## Read/Write Separation
- Service layer methods separate reads and writes:
  - e.g., `SearchAsync`/`GetByIdAsync` (reads) vs `CreateAsync`/`UpdateAsync`/`DeleteAsync` (writes).

## Azure Functions
- At least two functions are used:
  - `DailyBatch` (timer-based)
  - `ManualBatch` (HTTP trigger)
  - `GenerateOffer` (HTTP trigger)

## Testability
- Business logic is injected via DI and interfaces.
- Pure components (URL builders, mappers) are covered by unit tests.
- Service classes can be mocked or tested with in-memory/test doubles as needed.
