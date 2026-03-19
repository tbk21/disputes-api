# disputes-api

.NET 8 Web API for the Disputes Portal assessment.

## Features

- JWT authentication
- Transactions listing and filtering
- Raise disputes
- View dispute detail
- Dispute event history
- Seeded demo users and demo data
- Docker support for SQL Server and API

## Tech Stack

- .NET 8
- Entity Framework Core
- SQL Server
- Docker

## Demo Users

- customer1@demo.local / Password123!
- customer2@demo.local / Password123!
- customer3@demo.local / Password123!
- agent1@demo.local / Password123!
- admin@demo.local / Password123!

## Run SQL Server with Docker

```bash
docker compose up -d db