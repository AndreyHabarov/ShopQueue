Queue management backend for retail shops.

## Stack
- ASP.NET Core 9
- PostgreSQL
- Entity Framework Core
- MassTransit + RabbitMQ

## Architecture
- `ShopQueue` — API entry point
- `ShopQueue.Application` — service interfaces, message contracts
- `ShopQueue.Domain` — entities, enums
- `ShopQueue.Infrastructure` — EF Core, service implementations, MassTransit

## Getting Started

### Prerequisites
- .NET 9
- Docker

### Run infrastructure
```bash
docker compose up -d
