# 03 — Create Task: Unit Tests (Backend, TDD Red Phase)

You are a Senior .NET Engineer following Test-Driven Development (TDD).

The project follows Clean Architecture. The solution already contains:

- `TaskManager.Api`
- `TaskManager.Application`
- `TaskManager.Domain`
- `TaskManager.Infrastructure`
- `TaskManager.UnitTests`

## Your task

Write **only** the unit tests for the `CreateTask` use case.

Do **not** generate any production code.

The tests must **fail** because the production code does not exist yet (red phase).

## Interfaces and classes your tests will depend on (to be created as stubs)

You will need to define the following in the test project so tests compile:

| Type | Namespace | Description |
|---|---|---|
| `CreateTaskCommand` | `TaskManager.Application.Common.Cqrs` | Input command |
| `TaskDto` | `TaskManager.Application.Common.Cqrs` | Output DTO |
| `ITaskRepository` | `TaskManager.Application.Common.Cqrs` | Repository abstraction |
| `IUnitOfWork` | `TaskManager.Application.Common.Cqrs` | Unit of work abstraction |
| `CreateTaskHandler` | `TaskManager.Application.Common.Cqrs` | Handler under test |
| `ValidationException` | `TaskManager.Application.Common.Cqrs` | Thrown on invalid input |

## Business rules to test

- Title is required.
- Title maximum length is 200 characters.
- Description maximum length is 1000 characters.
- DueDate cannot be in the past.
- Status must be one of: `Pending`, `InProgress`, `Completed`.
- The `AuthenticatedUserId` on the command becomes the `OwnerId` of the created entity.
- Repository `AddAsync` must be called exactly once.
- `IUnitOfWork.SaveChangesAsync` must be called exactly once.
- The handler must return a `TaskDto` mapped from the created entity.

## Test scenarios (cover all of these)

1. `CreateTask_Success` — happy path returns a valid `TaskDto`.
2. `CreateTask_Fails_WhenTitleEmpty` — throws `ValidationException`.
3. `CreateTask_Fails_WhenTitleTooLong` — title with 201 characters throws.
4. `CreateTask_Fails_WhenDescriptionTooLong` — description with 1001 characters throws.
5. `CreateTask_Fails_WhenDueDateIsInThePast` — past `DueDate` throws.
6. `CreateTask_Fails_WhenStatusIsInvalid` — unrecognized status string throws.
7. `CreateTask_CallsRepository_ExactlyOnce` — verify `AddAsync` called once.
8. `CreateTask_CallsUnitOfWork_ExactlyOnce` — verify `SaveChangesAsync` called once.
9. `CreateTask_SetsOwnerIdFromAuthenticatedUser` — `OwnerId` equals `AuthenticatedUserId`.
10. `CreateTask_MapsResponseDto_Correctly` — all fields on DTO match command input.

## Requirements

Use:

- xUnit
- Moq
- Shouldly

Place tests in: `backend/tests/TaskManager.UnitTests/`

Add an XML comment on each test explaining **why** it exists.

The tests should **not** pass until the production code is implemented in the next prompt.