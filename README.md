# .NET Challenge

## 🎯 Purpose

This is a **deliberately simple** .NET 8 solution used for live coding interviews. The architecture is intentionally basic (Controller → Service → EF Core DbContext) with **buggy Kafka consumption** that candidates must diagnose and fix, then refactor into a more scalable structure.

## 📋 Prerequisites

- **.NET 8 SDK** ([download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Docker Desktop**
- Basic understanding of C#, ASP.NET Core, and Kafka

## 🚀 Quick Start

### 1. Start Infrastructure

From the root of the parent directory (where `kafka-db-docker-compose.yaml` is located):

```bash
docker compose -f kafka-db-docker-compose.yaml up -d
```

This starts:
- Zookeeper (port 2181)
- Kafka (port 9092)
- Kafka UI (port 8090) - visit http://localhost:8090
- MongoDB (port 27017) - for order persistence
- Mongo Express (port 8081) - visit http://localhost:8081 (admin/admin)
- **Automatically creates the `orders` topic with 3 partitions**
- **Automatically seeds 50 sample orders for the interview challenge**

The seeding happens automatically during Docker Compose startup! No manual steps needed.

### 2. Verify Messages in Kafka

```bash
docker exec -it kafka kafka-console-consumer --topic orders --bootstrap-server localhost:9092 --from-beginning --max-messages 10
```

You should see 50 orders total (ORD-000001 through ORD-000050) distributed across 3 partitions.

You should see 5 orders (ORD-000001 through ORD-000005).

### 3. Build the Solution

```bash
cd dotnet-streaming-challenge
dotnet build
```4

### 5. Run the Application

```bash
dotnet run --project src/WebApp/WebApp.csproj
```

The 5PI will start on `http://localhost:5100` (or check the console output).

### 4. Test the API

**Get all orders:**

```bash
curl http://localhost:5100/orders
```

**Get an order:**

```bash
curl http://localhost:5100/orders/ORD-NEW-001
```

### 6. Run Tests

```bash
dotnet test
```