# eCommerceSolution.ProductsService

A high-performance product catalog microservice built with **.NET Core**, optimized for low-latency read operations using **Redis caching** and structured persistence via **MS SQL Server**.

## 🛠️ Tech Stack & Infrastructure
* **Framework:** .NET Core API
* **Primary Database:** MS SQL Server (For structured, highly consistent product catalog data)
* **Caching Layer:** Distributed Redis Cache (Secured with password authentication for fast internal lookups)
* **Containerization:** Docker & Docker Compose
* **Network Isolation:** Databases run on an isolated `product-db-network`, communicating with internal microservices strictly over the shared `inter-service-network`.

## 🏗️ Architecture Role & Data Flow
This service handles product catalogs, stock listings, and inventory tracking.
* **Performance Engineering:** Integrated a **Distributed Redis Cache** layer to drastically minimize latency for high-frequency internal read workflows (e.g., when the Order Service needs to validate product details during a checkout flow).

## 📂 System Architecture Overview
This repository is part of a larger, decentralized microservice ecosystem:
1. **[UsersService](https://github.com/B3nzy/eCommerceSolution.UsersService)** (PostgreSQL)
2. **[ProductsService](https://github.com/B3nzy/eCommerceSolution.ProductsService)** (MS SQL Server + Redis) - *You are here*
3. **[OrdersService](https://github.com/B3nzy/eCommerceSolution.OrdersService)** (MongoDB)

## 🚀 How to Run (via Orchestrated Compose)
To run this service alongside the entire ecosystem, navigate to the root configuration containing the `docker-compose.yml` file and execute:
```bash
docker-compose up --build
