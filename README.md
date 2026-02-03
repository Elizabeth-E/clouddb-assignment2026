# Local Setup

This guide gets the full stack running locally: SQL Server, Azurite (Blob/Queue/Table), SQLPad, Azure Functions (batch + offer generation), and the Web API.

## Prerequisites
- Docker Desktop (or Docker Engine)
- .NET 8 SDK
- Optional: Azure Storage Explorer (for viewing Azurite blobs)

## 1) Start infrastructure + Functions
From the repo root:

```bash
docker compose up --build
```

This starts:
- SQL Server on `localhost:1433`
- Azurite on `localhost:10000/10001/10002`
- SQLPad on `http://localhost:3000`
- Functions on `http://localhost:7071`

## 2) Create the database + run migrations
The SQL container starts empty. Create the DB and apply migrations:

```bash
dotnet tool install --global dotnet-ef

dotnet ef database update \
  --project Data \
  --startup-project BuyMyHouseApi
```

If you already have `dotnet-ef`, you can skip the install step.

## 3) Run the Web API
In another terminal:

```bash
dotnet run --project BuyMyHouseApi
```

Swagger UI:
- `http://localhost:5000/swagger`

## 4) SQLPad connection
Open SQLPad at `http://localhost:3000` and create a connection:

- **Driver:** SQL Server
- **Host:** `sql`
- **Port:** `1433`
- **Database:** `BuyMyHouseDb`
- **Username:** `sa`
- **Password:** `YourStrong(!)Password123`
- **Encrypt/SSL:** Off (or Trust Server Certificate)

SQLPad runs inside Docker, so the host is `sql` (the container name), not `localhost`.

## 5) Run the manual batch
The manual batch endpoint is anonymous for local dev:

```bash
curl -X POST "http://localhost:7071/api/batch/run" \
  -H "Content-Type: application/json"
```

## 6) Seed sample houses (optional)
The API has a seed endpoint that inserts 5 houses:

```bash
curl -X POST "http://localhost:5000/api/houses/seed" \
  -H "Content-Type: application/json"
```

## 7) Viewing offer documents (blobs)
Offer documents are stored in Azurite and the API returns direct blob URLs.
Example URL format:

```
http://localhost:10000/devstoreaccount1/mortgage-offers/offers/<applicant>/<applicationId>/<offerId>.json
```

If you use Azure Storage Explorer, connect to the Azurite emulator using:

```
DefaultEndpointsProtocol=http;
AccountName=devstoreaccount1;
AccountKey=Eby8vdM02xNoGFp6m2I/3Nn3CkE7q8m0G6C+4xTQmG2jVv8aWl6Ue2DYON2SIgMZk1G8h8C8bL8aS7WqZbFj7Q==;
BlobEndpoint=http://localhost:10000/devstoreaccount1;
QueueEndpoint=http://localhost:10001/devstoreaccount1;
TableEndpoint=http://localhost:10002/devstoreaccount1;
```

## Troubleshooting
- If Functions can’t start, check:
  ```bash
  docker logs buy-my-house-functions --tail 200
  ```
- If SQLPad can’t connect, confirm the SQL container is healthy:
  ```bash
  docker compose ps
  ```
- If migrations fail, verify SQL is reachable at `localhost:1433` and the password matches `docker-compose.yml`.
