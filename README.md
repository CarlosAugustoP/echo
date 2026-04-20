# Echo: A Blockchain-Powered Social Donation Platform

Echo is a transparent donation platform that connects **Donors**, **NGOs**, and **Vendors** through Ethereum smart contracts. Donations are held in escrow on-chain and released only when funds are properly directed to verified suppliers or directly to NGOs, guaranteeing full traceability of every transaction.

The project is built for academic purposes, as an artifact for my bachelor's degree thesis: **From Trust to Impact: Applying Blockchain in Philanthropy**

---

## Purpose

Traditional donation platforms offer little visibility into where money actually goes. Echo solves this by:

- Recording every donation as an Ethereum transaction.
- Holding funds in a **Solidity escrow smart contract** until they are assigned to a trusted, admin-approved vendor or released directly to the NGO.
- Tracking each donation through a complete lifecycle with on-chain confirmations.
- Providing NGOs with a project management layer (goals, blog posts, vendor assignments) so donors can follow impact in real time.

---

## Architecture Overview

The solution is structured as a **.NET 9 DDD Clean Architecture** with four projects and a background worker, all orchestrated via Docker Compose.

```
EchoProject.sln
├── EchoProject.Api               # ASP.NET Core REST API (entry point)
├── EchoProject.Application       # System flow, services, DTOs, validators
├── EchoProject.Domain            # Business logic, Domain entities, aggregates, value objects
├── EchoProject.Infrastructure    # EF Core/PostgreSQL, Nethereum, Supabase, Rebus
├── EchoProject.BlockchainWorker  # Background service for blockchain polling
└── EchoProjectScrow.sol          # Solidity escrow smart contract
```

### Project Layers

| Project | Responsibility |
|---|---|
| **Api** | HTTP controllers, middleware, Swagger, JWT auth pipeline |
| **Application** | Application services, AutoMapper profiles, FluentValidation, request/DTO models, Rebus event consumers |
| **Domain** | Aggregates (`User`, `Project`, `Goal`, `Donation`, `Vendor`), domain exceptions, repository interfaces, value objects |
| **Infrastructure** | EF Core `DbContext`, PostgreSQL migrations, Nethereum (Ethereum), Supabase file storage, Rebus/RabbitMQ setup, Unit of Work |
| **BlockchainWorker** | .NET `BackgroundService` that polls pending donations every 5 minutes and publishes status-update events |

---

## Domain Model

### User Roles

| Role | Description |
|---|---|
| `Donor` | Authenticates and sends ETH donations to project goals |
| `NGO` | Creates and manages projects, assigns vendors, publishes blog posts |
| `EchoAdmin` | Platform administrator; approves or rejects vendor applications |

### Core Aggregates

**Project** — Created by an NGO. Contains a list of **Goals**, blog posts, and a smart contract address where donations are held.

**Goal** — Belongs to a project. Has a `GoalType` (e.g., *Money*, food, medicine, etc.), a target amount, a current amount, and optionally a cost-per-unit and assigned vendors.

- *Money goals* transfer ETH directly to the NGO's wallet.
- *Vendor goals* hold funds in escrow until the NGO assigns a donation to an approved vendor.

**Donation** — Created when a donor submits an Ethereum transaction hash. Progresses through a status lifecycle:

```
TransferredToContract
  → ImmediateTransferToNGOInContract  → ImmediateTransferToNGOConfirmed
  → TransferredToVendorPending        → TransferredToVendorConfirmed
  → Failed | ExpiredAndRefunded
```

**Vendor** — A trusted supplier registered by an NGO. Must be approved by an `EchoAdmin` before receiving funds. Can be assigned to specific goals.

---

## API Endpoints

All routes are prefixed with `/api`. JWT Bearer authentication is required where noted.

### Auth — `/api/auth`
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/signup` | — | Register a new Donor or NGO account |
| POST | `/login` | — | Authenticate and receive a JWT token |
| POST | `/me` | ✅ Any | Get the current user's profile |

### Projects — `/api/projects`
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/trending` | — | Paginated list of trending projects by total donations |
| GET | `/for-you` | ✅ Donor | Personalized project recommendations |
| GET | `/{id}` | — | Get a project by ID |
| GET | `/manager/{managerId}` | ✅ Any | Projects managed by a specific NGO |
| POST | `/` | ✅ NGO | Create a new project |
| PUT | `/{id}` | ✅ NGO | Update project title/description |
| POST | `/{id}/goals` | ✅ NGO | Add a goal to a project |
| DELETE | `/{id}/goals/{goalId}` | ✅ NGO | Remove a goal |
| POST | `/blog-post/{projectId}` | ✅ NGO | Publish a blog post |
| GET | `/blog-post/{blogPostId}` | — | Get a blog post |
| GET | `/blog-posts/{projectId}` | — | Paginated blog posts for a project |
| POST | `/blog-post/{projectId}/{blogPostId}/add-image` | ✅ NGO | Attach an image to a blog post |

### Donations — `/api/donations`
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/donate` | ✅ Donor | Submit a donation (ETH tx hash) to a goal |
| GET | `/history` | ✅ Donor | Paginated personal donation history |
| GET | `/view-donation/{id}` | ✅ Any | Get donation details |
| GET | `/project/{projectId}` | ✅ NGO/Admin | Donations for a project |
| POST | `/transfer-to-vendor/{donationId}/{vendorId}` | ✅ NGO | Release escrowed funds to a vendor |
| GET | `/timeline/{donationId}` | ✅ Donor | Full event timeline for a donation |
| GET | `/donation-distribution` | — | Global donation breakdown by goal type |

### Vendors — `/api/vendors`
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/` | ✅ NGO | Submit a vendor application |
| GET | `/{vendorId}` | — | Get vendor details |
| POST | `/approve/{vendorId}` | ✅ EchoAdmin | Approve a vendor |
| POST | `/deny/{vendorId}` | ✅ EchoAdmin | Reject a vendor |
| POST | `/vendor/{vendorId}/goal/{goalId}` | ✅ NGO | Assign a vendor to a goal |

### Wallet — `/api/wallet`
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/balance/{walletAddress}` | — | Get ETH balance of any wallet address |

### User Profile
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/echo-amount` | ✅ Donor/NGO | Get user's Echo points |
| PATCH | `/` | ✅ Donor/NGO | Update user profile |
| PATCH | `/wallet-address` | ✅ Donor/NGO | Update linked Ethereum wallet address |

---

## Smart Contract

**`EchoProjectScrow.sol`** is a Solidity (^0.8.18) escrow contract deployed per project.

- **`receive()`** — Accepts ETH from donors and emits a `DonationReceived` event.
- **`releaseFunds(address payable _supplier, uint256 _amount)`** — Callable only by the platform admin (API wallet); transfers funds to the vendor and emits `FundsReleased`.
- **`getBalance()`** — Returns the current ETH balance held in escrow.

---

## Blockchain Worker

`EchoProject.BlockchainWorker` is a .NET `BackgroundService` that runs every **5 minutes**:

1. Queries the database for donations in pending states.
2. For **vendor-pending** donations: checks the Ethereum transaction on-chain; if confirmed, publishes a `DonationStatusUpdatedMessage` to RabbitMQ.
3. For **direct NGO** donations: calls `releaseFunds` on the smart contract to transfer ETH to the NGO's wallet, then publishes the confirmation event.

The API subscribes to these events via **Rebus** and updates donation statuses accordingly.

---

## Key Libraries

| Library | Purpose |
|---|---|
| **ASP.NET Core 9** | Web API framework |
| **Entity Framework Core 9** + **Npgsql** | ORM with PostgreSQL database |
| **Nethereum** | .NET Ethereum client (read/write blockchain, call smart contracts) |
| **Rebus** + **Rebus.RabbitMq** | Async message bus; decouples the blockchain worker from the API |
| **Supabase** | File/image storage for blog post media |
| **AutoMapper** | Object-to-object mapping between domain entities and DTOs |
| **FluentValidation** | Request model validation |
| **JWT Bearer** + **BCrypt.Net** | Authentication and password hashing |
| **RabbitMQ** | Message broker (managed via Docker) |

---

## Running Locally

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- A PostgreSQL instance and Ethereum RPC endpoint (e.g., Infura)

### With Docker Compose

```bash
docker compose up --build
```

This starts:
- **RabbitMQ** on ports `5672` (AMQP) and `15672` (management UI)
- **echo-api** on port `5087`
- **echo-worker** (blockchain background service)

Compose files are split by responsibility:
- `docker-compose.yml`: stable service definitions, shared environment-variable wiring, images, and network
- `docker-compose.override.yml`: local Docker build, localhost port mappings, and local-only mounts
- `docker-compose.prod.yml`: production image tags for CI/CD and no localhost port publishing

The production secret contract mirrors the `EchoProject.Api/appsettings.json` structure through .NET environment-variable binding. GitHub Actions pushes both images to ACR and deploys `echo-api` and `echo-worker` to Azure Container Apps, updating the app secrets and environment variables directly from repository secrets and variables.

Required GitHub repository **secrets**:

- `ACR_USERNAME`, `ACR_PASSWORD` -> credentials used by the workflow to log in to `echoappregistry.azurecr.io` and push the `echo-api` and `echo-worker` images
- `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID` -> Azure login used by the workflow to update the Container Apps
- `ECHO_CONNECTION_STRING` -> `ConnectionStrings:DefaultConnection`
- `BLOCKCHAIN_RPC_URL` -> `BlockChainSettings:RpcUrl`
- `BLOCKCHAIN_PRIVATE_KEY` -> `BlockChainSettings:EthereumPrivateKey`
- `BLOCKCHAIN_ACCOUNT_ADDRESS` -> `BlockChainSettings:EthereumAccountAddress`
- `JWT_SECRET_KEY` -> `JwtSettings:SecretKey`
- `AUTOMAPPER_LICENSE_KEY` -> `AutoMapper:LicenseKey`
- `SUPABASE_URL` -> `Supabase:Url`
- `SUPABASE_KEY` -> `Supabase:Key`
- `RABBITMQ_USERNAME` -> `RabbitMqSettings:Username`
- `RABBITMQ_PASSWORD` -> `RabbitMqSettings:Password`

Required GitHub repository **variables**:

- `AZURE_RESOURCE_GROUP` -> resource group that contains the Azure Container Apps
- `API_CONTAINER_APP_NAME` -> Azure Container App name for the API
- `WORKER_CONTAINER_APP_NAME` -> Azure Container App name for the blockchain worker

Optional GitHub repository **variables** with safe defaults:

- `BLOCKCHAIN_CHAIN_ID` -> default `11155111`
- `JWT_EXPIRATION_HOURS` -> default `8`
- `JWT_ISSUER` -> default `EchoProject`
- `SUPABASE_BUCKET_NAME` -> default `echo-public-bucket`
- `RABBITMQ_HOST` -> default `rabbitmq`
- `RABBITMQ_VHOST` -> default `/`

The workflow in `.github/workflows/prepare-deploy.yml`:

- builds and pushes `echo-api` and `echo-worker` to `echoappregistry.azurecr.io`
- configures ACR pull credentials on both Azure Container Apps
- updates the Azure Container App secrets from GitHub repository secrets
- deploys new single-revision releases for both apps with the new image tags and runtime environment variables

### Database Migrations

```powershell
# Create a new migration
.\migrate-db.ps1

# Apply pending migrations
.\update-db.ps1
```

### Swagger UI

When running in `Development` mode, the interactive API docs are available at:
```
http://localhost:5087/swagger
```

---

## Pagination

All paginated endpoints accept the following query parameters:

| Parameter | Default | Description |
|---|---|---|
| `pageNumber` | `0` | Zero-based page index |
| `pageSize` | *(configured)* | Number of results per page |
