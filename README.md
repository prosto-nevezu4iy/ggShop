# 🕹️ ggShop — Video Game Store Platform (Microservices Architecture)

A modern, scalable **e-commerce platform for video games**, built with **.NET 8** and a **microservices architecture**.  
Each service is independently deployable, containerized with **Docker**, and communicates through **RabbitMQ** events.

---

## 🚀 Architecture Overview

The platform handles various aspects of an online store, such as product catalog management, user authentication, shopping cart, payment processing, and order management. Each of these features are implemented as separate microservices, allowing for independent development, deployment, and scaling.


---

## 🧩 Core Microservices

### 🧑 Identity Service
- Based on **Duende IdentityServer + ASP.NET Identity**
- Handles:
  - User registration & login
  - JWT / OpenID Connect token issuing
  - Password reset via **Brevo**
  - User profile management (name, surname, date of birth and so on)

### 🎮 Catalog Service
- Manages games, genres, publishers, and platforms
- Built on **.NET 9 + PostgreSQL + EF Core**
- Supports:
  - Full-text search on name/description
  - Filtering & sorting by price, discount, genre, platform, publisher, availability
  - Cloudinary images management
- Uses **Outbox Pattern** for reliable event publishing

### 🛒 Shopping Cart Service
- Built with **.NET 9** and **Redis**
- Supports both **anonymous** and **authenticated** carts
- Automatically merges anonymous cart into user cart
- Uses RabbitMQ for event-driven communication

### 📦 Order Service
- Built on **.NET 9 + PostgreSQL + EF Core**
- Processes orders
- Integrates with Catalog and ShoppingCart Service
- Uses RabbitMQ to listen for checkout events

### 💳 Payment Service *(planned)*
- Handles payments via external providers (Stripe)

### ✉️ Notification Service *(planned)*
- Sends transactional emails (Brevo)
  - Registration confirmation
  - Password reset
  - Order confirmations
- Listens for domain events via RabbitMQ

---

## ⚙️ Additional Components — ggShop Infrastructure

The **ggShop Video Game Store** platform uses several **infrastructure components** that support scalability, fault tolerance, and efficient operations across all microservices.  
These components ensure the system is **observable**, **manageable**, and **resilient** in production environments.

---

## 🧭 Overview

| Component | Purpose | Technology |
|------------|----------|-------------|
| API Gateway | Central entry point for client requests | YARP |
| Service Discovery | Dynamic service registration & discovery | Consul / Eureka |
| Centralized Logging | Unified logs from all microservices | ELK Stack (Elasticsearch, Logstash, Kibana) |
| Containerization | Isolated, portable service deployments | Docker |
| Orchestration | Define and run multi-service setups | Docker Compose / Kubernetes |
| CI/CD | Automated build, test & deployment pipeline | GitHub Actions / GitLab CI |
| Monitoring | Track performance, uptime, and metrics | Prometheus / Grafana |

---

## ⚙️ Technologies Used

| Category | Technology |
|-----------|-------------|
| Language | C# (.NET 9) |
| API Communication | REST |
| Service Communication | GRPC |
| Database | PostgreSQL (Catalog, Orders) |
| Cache / Store | Redis (Shopping Cart) |
| Messaging | RabbitMQ (MassTransit) |
| Authentication | Duende IdentityServer + ASP.NET Identity |
| Image Storage | Cloudinary |
| Email Service | Brevo |
| Frontend | Svelte
| Logging | Serilog |
| Containerization | Docker |

---

## 🧠 Future Plans
- Add Common project and implement Global Handling and Result pattern
- Use centralized place for access user identity
- Refactor all services using last language features
- Implement Payment service
- Add code coverage for all services
- Implement RBAC
- Add Admin Panel for Catalog management
- Deploy on Kubernetes with auto-scaling

## 🧰 Development Setup

### 1️⃣ Clone the repository
```bash
git clone https://github.com/yourusername/ggShop.git
cd ggShop
