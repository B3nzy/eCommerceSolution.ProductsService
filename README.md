# eCommerceSolution.ProductsService

A high-performance product catalog microservice built with **.NET Core**, optimized for low-latency read operations using **Redis caching** and structured persistence via **MS SQL Server**.

## 🛠️ Tech Stack & Infrastructure

* **Framework:** .NET Core API
* **Primary Database:** MS SQL Server (For structured, highly consistent product catalog data)
* **Caching Layer:** Distributed Redis Cache (Secured with password authentication for fast internal lookups)
* **Message Broker:** RabbitMQ (via MassTransit for event publishing)
* **Containerization:** Docker & Docker Compose
* **Network Isolation:** Databases run on an isolated `product-db-network`, communicating with internal microservices strictly over the shared `inter-service-network`.

## 🏗️ Architecture Role & Data Flow

This service acts as the single source of truth for core product catalog data, managing product details, categorization, descriptions, and base pricing. 

*(Note: Stock management, deductions, and inventory tracking have been fully decoupled from this service and are now handled asynchronously by the dedicated **Inventory Service**).*

* **Performance Engineering:** Integrated a **Distributed Redis Cache** layer to drastically minimize latency for high-frequency internal read workflows (e.g., when the Order Service or external API Gateway needs to rapidly validate product details during a checkout flow).

## 📂 System Architecture Overview

This repository is part of a larger, decentralized microservice ecosystem:

1. **[UsersService](https://github.com/B3nzy/eCommerceSolution.UsersService)** (PostgreSQL)
2. **[ProductsService](https://github.com/B3nzy/eCommerceSolution.ProductsService)** (MS SQL Server + Redis) - *You are here*
3. **[OrdersService](https://github.com/B3nzy/eCommerceSolution.OrdersService)** (MongoDB)
4. **[InventoryService](https://github.com/B3nzy/eCommerceSolution.InventoryService)** (MS SQL Server)

## 🚀 How to Run (via Orchestrated Compose)

To run this service alongside the entire ecosystem, navigate to the root configuration containing the `docker-compose.yml` file and execute:

```bash
docker-compose up --build -d
