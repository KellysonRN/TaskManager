You are a Senior Software Architect.

Create a production-ready monorepo for a full-stack Task Management application.

The repository must follow Clean Architecture principles and be easy to present during a technical interview.

The monorepo must contain the following structure:

./
│
├── backend/
│   ├── TaskManager.slnx
│   ├── src/
│   │   ├── TaskManager.Api
│   │   ├── TaskManager.Application
│   │   ├── TaskManager.Domain
│   │   └── TaskManager.Infrastructure
│   │
│   ├── tests/
│   │   ├── TaskManager.UnitTests
│   │   └── TaskManager.IntegrationTests
│   │
│   └── docker/
│
├── frontend/
│   ├── src/
│   ├── public/
│   ├── package.json
│   ├── angular.json
│   └── ...
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

Requirements

- .NET 10
- ASP.NET Core Web API
- Angular 20
- SQLite
- Entity Framework Core
- JWT Authentication
- Swagger
- FluentValidation
- Shouldly
- xUnit
- Moq
- Docker
- Docker Compose

The backend must follow Clean Architecture.

Domain
Application
Infrastructure
Presentation

must be completely separated.

Create folders only.

Do not implement business logic yet.

Create a professional README with setup instructions.

Generate all necessary configuration files.