# 01 — Monorepo Scaffold

You are a Senior Software Architect.

Create a production-ready monorepo for a full-stack Task Management application.

The repository must follow Clean Architecture principles and be easy to present during a technical interview.

## Expected folder structure

```
./
│
├── backend/
│   ├── TaskManager.sln
│   ├── src/
│   │   ├── TaskManager.Api/
│   │   │   └── Dockerfile
│   │   ├── TaskManager.Application/
│   │   ├── TaskManager.Domain/
│   │   └── TaskManager.Infrastructure/
│   │
│   └── tests/
│       ├── TaskManager.UnitTests/
│       └── TaskManager.IntegrationTests/
│
├── frontend/
│   ├── src/
│   ├── package.json
│   ├── angular.json
│   └── Dockerfile
│
├── docs/
│   └── prompts/
│
├── scripts/
│   ├── setup.sh
│   ├── start-backend.sh
│   ├── start-frontend.sh
│   └── seed.sh
│
├── docker-compose.yml
├── README.md
├── AI_USAGE.md
├── .editorconfig
├── .gitignore
└── LICENSE
```

## Tech stack

- .NET 10 / ASP.NET Core Web API
- Angular 20 (standalone components)
- SQLite + Entity Framework Core
- JWT Authentication
- Swagger / OpenAPI
- FluentValidation
- Shouldly
- xUnit
- Moq
- Docker + Docker Compose

## Clean Architecture layers — must be completely separated

| Layer | Project |
|---|---|
| Domain | TaskManager.Domain |
| Application | TaskManager.Application |
| Infrastructure | TaskManager.Infrastructure |
| Presentation | TaskManager.Api |

## Constraints

- Create **folder and project scaffolding only**.
- Do **not** implement any business logic.
- Do **not** add placeholder classes beyond what is needed for the solution to compile.
- Create a professional `README.md` with prerequisites, setup instructions, and how to run each part.
- Create `.editorconfig` and `.gitignore` appropriate for a .NET + Angular monorepo.
- Generate all necessary configuration files (`appsettings.json`, `angular.json`, `docker-compose.yml`, etc.).
