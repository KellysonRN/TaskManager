# Task Manager Monorepo

A production-ready monorepo for a full-stack Task Management application built with Clean Architecture principles and designed for technical interviews.

## Overview

This repository provides a scalable starting point for:

- a .NET 10 ASP.NET Core Web API backend
- an Angular 20 frontend
- SQLite persistence with Entity Framework Core
- JWT-based authentication
- Swagger documentation
- Docker and Docker Compose support

## Architecture

The backend follows Clean Architecture with clear separation between:

- Domain
- Application
- Infrastructure
- Presentation

## Repository Structure

- backend/: ASP.NET Core solution and projects
- frontend/: Angular 20 application
- docs/: architecture notes, prompts, ADRs, diagrams, and presentation materials
- scripts/: setup and startup automation

## Prerequisites

- .NET 10 SDK
- Node.js 20+
- Docker Desktop
- Git

## Getting Started

### 1. Clone and enter the monorepo

```bash
git clone https://github.com/KellysonRN/TaskManager.git
cd TaskManager
```

### 2. Run the setup script

```bash
chmod +x ./scripts/setup.sh ./scripts/start-backend.sh ./scripts/start-frontend.sh ./scripts/seed.sh
./scripts/setup.sh
```

### 3. Start the backend

```bash
./scripts/start-backend.sh
```

### 4. Start the frontend

```bash
./scripts/start-frontend.sh
```

### 5. Seed the database (optional)

```bash
./scripts/seed.sh
```

### 6. Run with Docker Compose

```bash
docker compose up --build
```

### 7. Verify services are up (Docker)

After bringing the stack up, use these commands to confirm services started and are serving content:

- Build and start frontend only (no cache):

```bash
docker compose build --no-cache frontend
docker compose up -d frontend
```

- Check running containers:

```bash
docker compose ps
```

- View recent logs (useful for build/runtime errors):

```bash
docker compose logs --tail=200 frontend
docker compose logs --tail=200 backend
```

- Confirm frontend serves the Angular app (open in browser):

	- http://localhost:4200

- Confirm backend health endpoint (open in browser or curl):

```bash
curl -sS http://localhost:5000/health | jq .
# or simple check
curl -I http://localhost:5000/health
```

- Inspect files inside the frontend container to verify the built artifacts and nginx config:

```bash
docker compose exec frontend ls -la /usr/share/nginx/html
docker compose exec frontend sed -n '1,120p' /usr/share/nginx/html/index.html
docker compose exec frontend cat /etc/nginx/conf.d/default.conf
```

- If the Nginx default welcome page appears, verify `index.html` exists in `/usr/share/nginx/html` and that `nginx.conf` was copied into `/etc/nginx/conf.d/default.conf`.

- To stop and remove containers (and volumes):

```bash
docker compose down -v
```


## Technology Stack

- Backend: ASP.NET Core Web API, .NET 10, EF Core, SQLite, JWT, Swagger, FluentValidation
- Frontend: Angular 20, TypeScript, RxJS
- Testing: xUnit, Moq, Shouldly
- Containerization: Docker, Docker Compose

## Notes

This scaffold intentionally focuses on structure, configuration, and developer experience. Business logic and domain features will be implemented in subsequent iterations.
