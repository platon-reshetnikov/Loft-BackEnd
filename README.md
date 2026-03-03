# 🛒 Loft Shop — Ukrainian E-Commerce Marketplace

> A full-featured classifieds marketplace inspired by **OLX** and **Amazon**. The platform allows sellers to list products and buyers to browse, add to cart, place orders, and pay online.

**🌐 Production:** [https://loft-shop.pp.ua](https://loft-shop.pp.ua)

---

## 👥 Team

The project was developed by a team of **3 backend developers** in close collaboration with several other disciplines:

| Role | Responsibilities |
|------|-----------------|
| ⚙️ Backend (×3) | Microservice architecture on .NET 8, business logic, REST API, database |
| 🚀 DevOps | CI/CD pipeline (GitHub Actions), Docker Hub, deployment to Linux server |
| 🎨 Design | UI/UX page design, responsive layouts |
| 💻 Frontend | React application, REST API & SignalR chat integration |

> **Infrastructure:** the project is hosted on a **Linux server** which also runs the **PostgreSQL** database. The domain `loft-shop.pp.ua` with an SSL certificate is served via **Nginx** as a reverse proxy.

---

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Microservice Architecture](#-microservice-architecture)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Running the Project](#-running-the-project)
- [CI/CD & Infrastructure](#-cicd--infrastructure)
- [Environment Variables](#-environment-variables)
- [Repository Structure](#-repository-structure)

---

## 🏪 About the Project

**Loft Shop** is a classifieds marketplace in the style of OLX with e-commerce elements inspired by Amazon. The project covers the full cycle: from user registration and product listing by a seller — to placing an order, payment, and delivery management by a buyer.

**Key features:**
- 🔐 JWT authentication + **Google OAuth 2.0**
- 👤 Three roles: `CUSTOMER`, `SELLER`, `MODERATOR`
- 📦 Two product types: **Physical** and **Digital**
- 🖼️ Media service with automatic thumbnail generation
- 💬 Real-time chat between seller and buyer (SignalR)
- ❤️ Favorites list
- 💳 Payment via **Stripe**, credit card, or cash on delivery
- 🛡️ Listing moderation (`Pending` → `Approved` / `Rejected`)
- 📧 Email notifications (Zoho SMTP)

---

## 🏗 Microservice Architecture

The system is built on **microservice architecture** principles. All external requests pass through a single entry point — the **Ocelot API Gateway**.

```
                        ┌──────────────────┐
                        │     Internet     │
                        └────────┬─────────┘
                                 │ HTTPS
                        ┌────────▼─────────┐
                        │  Nginx (Reverse  │
                        │  Proxy + SSL)    │
                        │  loft-shop.pp.ua │
                        └────────┬─────────┘
                                 │ HTTP :5000
                        ┌────────▼─────────┐
                        │   API Gateway    │
                        │  (Ocelot :8080)  │
                        └──────┬──┬──┬─────┘
           ┌──────────┬────────┘  │  └────────┬──────────┐
           │          │           │           │          │
    ┌──────▼──┐  ┌────▼────┐ ┌───▼────┐ ┌───▼────┐ ┌───▼─────┐
    │  User   │  │Product  │ │ Cart   │ │ Order  │ │Payment  │
    │Service  │  │Service  │ │Service │ │Service │ │Service  │
    │  :5004  │  │  :5002  │ │ :5003  │ │ :5001  │ │  :5006  │
    └──────┬──┘  └────┬────┘ └───┬────┘ └───┬────┘ └───┬─────┘
           │          │           │           │          │
    ┌──────▼──┐  ┌────▼────┐     │      ┌────▼───┐      │
    │Shipping │  │ Media   │     │      │Shipping│      │
    │Address  │  │Service  │     │      │Address │      │
    │  :5005  │  │  :5008  │     │      │  :5005 │      │
    └─────────┘  └─────────┘     │      └────────┘      │
                                 │                       │
                        ┌────────▼───────────────────────▼─┐
                        │         PostgreSQL 15             │
                        │   (single DB on Linux server)     │
                        └──────────────────────────────────┘
```

### Services & Ports

| Service | Port | Description |
|---------|------|-------------|
| **ApiGateway** | `5000` | Single entry point, routing via Ocelot |
| **UserService** | `5004` | Registration, auth, profile, chat, favorites |
| **ProductService** | `5002` | Products, categories, attributes, moderation, comments |
| **CartService** | `5003` | Shopping cart |
| **OrderService** | `5001` | Orders, delivery statuses |
| **PaymentService** | `5006` | Payments (Stripe / card / cash) |
| **ShippingAddressService** | `5005` | User delivery addresses |
| **MediaService** | `5008` | Image storage and processing |

---

## ✨ Features

### 👤 UserService
- Registration and login (email + password)
- OAuth 2.0 via **Google**
- Password reset via email
- Profile: name, avatar, phone number
- Roles: `CUSTOMER` — buyer, `SELLER` — seller, `MODERATOR` — moderator
- Public seller profile
- **Favorites** — add/remove products
- **Real-time chat** between users via **SignalR**

### 📦 ProductService
- Post listings with photos, description, price, and currency (UAH/USD/EUR)
- Product types: `Physical` and `Digital`
- **Categories** with dynamic **attributes** (like OLX: size, color, condition, etc.)
- View counter
- Product comments
- **Moderation**: new listings start as `Pending`, a moderator approves (`Approved`) or rejects (`Rejected`)

### 🛒 CartService
- Shopping cart with products
- Validates product availability and prices via inter-service requests

### 📋 OrderService
- Place orders from the cart
- Statuses: `PENDING` → `PAID` → `SHIPPED` → `DELIVERED` / `CANCELED`
- Stores product snapshot at the time of order (OrderItems)

### 💳 PaymentService
- **STRIPE** — online payment via Stripe API (Test Mode), PaymentIntent
- **CREDIT_CARD** — simulated direct card payment
- **CASH_ON_DELIVERY** — pay cash upon delivery
- Statuses: `PENDING` → `REQUIRES_CONFIRMATION` → `COMPLETED` / `FAILED` / `REFUNDED`

### 🏠 ShippingAddressService
- Store and manage user delivery addresses

### 🖼️ MediaService
- Upload images (JPG, PNG, GIF, WebP)
- Automatic **thumbnail** generation via `SixLabors.ImageSharp`
- Categories: `avatars`, `products`, `general`
- Private and public file storage

---

## 🛠 Tech Stack

### Backend
| Technology | Purpose |
|------------|---------|
| **.NET 8 / ASP.NET Core** | Main framework |
| **Entity Framework Core** | ORM + Migrations |
| **PostgreSQL 15** | Database |
| **Ocelot** | API Gateway / routing |
| **SignalR** | Real-time chat |
| **JWT Bearer** | Authentication |
| **Google OAuth 2.0** | Social login |
| **Stripe.net** | Online payments |
| **SixLabors.ImageSharp** | Image processing |
| **AutoMapper** | DTO ↔ Entity mapping |
| **Swagger / OpenAPI** | API documentation |

### Infrastructure & DevOps
| Technology | Purpose |
|------------|---------|
| **Docker + Docker Compose** | Microservice containerization |
| **Docker Hub** | Image registry |
| **GitHub Actions** | CI/CD pipeline |
| **Nginx** | Reverse proxy, SSL termination |
| **Linux Server** | Production hosting (DB + all services) |
| **Zoho SMTP** | Email delivery |

---

## 🚀 Running the Project

### Requirements
- Docker & Docker Compose
- Git

### Local Development

```bash
# 1. Clone the repository
git clone https://github.com/your-org/loft-backend.git
cd loft-backend

# 2. Copy and configure environment variables
cp .env.production.example .env
# Fill in .env (minimum: STRIPE_SECRET_KEY)

# 3. Start all services
docker compose up --build

# API Gateway available at http://localhost:5000
# Swagger for each service: http://localhost:{PORT}/swagger
```

### Production Deployment

```bash
# Uses pre-built images from Docker Hub
docker compose -f compose.production.yaml up -d
```

> For production, make sure all environment variables are set (see below).

---

## 🔄 CI/CD & Infrastructure

### GitHub Actions Pipelines

| File | Purpose |
|------|---------|
| `.github/workflows/deploy.yml` | Simple SSH deploy to server |
| `.github/workflows/deploy-with-registry.yml` | Build images → push to Docker Hub → deploy |

### Deployment Flow

```
git push origin main
       │
       ▼
GitHub Actions
  ├─ Build Docker images
  ├─ Push to Docker Hub
  │    loft-apigateway, loft-userservice,
  │    loft-productservice, loft-cartservice,
  │    loft-orderservice, loft-paymentservice,
  │    loft-shippingaddressservice, loft-mediaservice
  └─ SSH → Linux Server
       └─ docker compose -f compose.production.yaml pull && up -d
```

### Server Infrastructure

- **Linux server** — hosts all Docker containers
- **PostgreSQL 15** — running on the same server in a Docker container
- **Nginx** — reverse proxy for `loft-shop.pp.ua` + SSL (`loft-shop.pp.ua.crt`)
- Automated server setup script: [`scripts/setup-server.sh`](scripts/setup-server.sh)
- Deployment script: [`scripts/deploy.sh`](scripts/deploy.sh)

### GitHub Secrets (required for CI/CD)

| Secret | Description |
|--------|-------------|
| `SSH_PRIVATE_KEY` | Private SSH key for server access |
| `SERVER_HOST` | Linux server IP address |
| `SERVER_USER` | SSH user |
| `DEPLOY_PATH` | Deploy path on server (e.g. `/opt/loft-backend`) |
| `DOCKER_USERNAME` | Docker Hub username |
| `POSTGRES_PASSWORD` | PostgreSQL password |
| `STRIPE_SECRET_KEY` | Stripe Secret Key |
| `STRIPE_PUBLISHABLE_KEY` | Stripe Publishable Key |

---

## ⚙️ Environment Variables

### Required to Run

```env
# Database
POSTGRES_PASSWORD=your_secure_password
POSTGRES_USER=developer
POSTGRES_DB=test_loft_shop

# Stripe (Payments)
STRIPE_SECRET_KEY=sk_test_...
STRIPE_PUBLISHABLE_KEY=pk_test_...
STRIPE_WEBHOOK_SECRET=whsec_...

# Google OAuth
GOOGLE_CLIENT_ID=...apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=GOCSPX-...

# Docker Hub (for CI/CD)
DOCKER_USERNAME=your_dockerhub_username
```

### JWT (default for local development)
```env
JWT_KEY=DevKey_ChangeMe_ForLocalOnly_1234567890
JWT_ISSUER=LoftUserService
JWT_AUDIENCE=LoftUsers
```

> ⚠️ **Never use dev keys in production!**

---

## 📁 Repository Structure

```
├── src/
│   ├── ApiGateway/             # Ocelot API Gateway
│   ├── UserService/            # Auth, profile, chat
│   ├── ProductService/         # Products, categories, moderation
│   ├── CartService/            # Shopping cart
│   ├── OrderService/           # Orders
│   ├── PaymentService/         # Payments (Stripe)
│   ├── ShippingAddressService/ # Delivery addresses
│   ├── MediaService/           # Media files
│   └── Common/Loft.Common/     # Shared Enums, DTOs
├── nginx/config/               # Nginx configuration
├── scripts/                    # Deploy & setup scripts
├── ssl/                        # SSL certificates
├── compose.yaml                # Docker Compose (local)
└── compose.production.yaml     # Docker Compose (production)
```

---

## 📚 Additional Documentation

- [💳 PaymentService README](src/PaymentService/README.md) — detailed Stripe integration guide
- [🖼️ MediaService README](src/MediaService/README.md) — media service API reference
- [🚀 CI/CD Setup](CI_CD_SETUP.md) — pipeline setup in 5 minutes
- [🔒 Security Policy](SECURITY.md) — reporting vulnerabilities
