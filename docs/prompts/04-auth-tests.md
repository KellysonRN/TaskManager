# 04 — Authentication: Unit Tests (Backend, TDD Red Phase)

You are a Senior .NET Engineer following Test-Driven Development (TDD).

The project follows Clean Architecture. The solution already contains:

- `TaskManager.Api`
- `TaskManager.Application`
- `TaskManager.Domain`
- `TaskManager.Infrastructure`
- `TaskManager.UnitTests`

## Your task

Write **only** the unit tests for the `Login` use case.

Do **not** generate any production code.

The tests must **fail** because the production code does not exist yet (red phase).

## Feature scope

The authentication flow is:

1. Client sends `POST /api/auth/login` with `{ email, password }`.
2. Backend finds the user by email.
3. Backend verifies the password hash.
4. Backend generates and returns a signed JWT.
5. Client stores the JWT and attaches it as `Authorization: Bearer <token>` on subsequent requests.

## Interfaces and classes your tests will depend on (stubs — to be created as minimal definitions)

| Type | Namespace | Description |
|---|---|---|
| `LoginCommand` | `TaskManager.Application.Auth.Commands.Login` | Input: `Email`, `Password` |
| `LoginResult` | `TaskManager.Application.Auth.Commands.Login` | Output: `Token` (string) |
| `LoginHandler` | `TaskManager.Application.Auth.Commands.Login` | Handler under test |
| `IUserRepository` | `TaskManager.Application.Auth.Contracts` | `FindByEmailAsync(email)` |
| `IPasswordHasher` | `TaskManager.Application.Auth.Contracts` | `Verify(plain, hash)` → bool |
| `ITokenService` | `TaskManager.Application.Contracts` | Already exists — generates JWT |
| `UserEntity` | `TaskManager.Domain` | `Id`, `Email`, `PasswordHash` |
| `ValidationException` | `TaskManager.Application.Common.Exceptions` | Already exists |
| `UnauthorizedException` | `TaskManager.Application.Common.Exceptions` | New — thrown on bad credentials |

## Business rules to test

- Email is required.
- Password is required.
- If no user is found with the given email → throw `UnauthorizedException`.
- If the password does not match the stored hash → throw `UnauthorizedException`.
- On success → call `ITokenService.GenerateToken` exactly once.
- On success → return a `LoginResult` with a non-empty `Token`.

## Test scenarios (cover all of these)

| # | Test name | Expected outcome |
|---|---|---|
| 1 | `Login_Success` | Returns `LoginResult` with non-empty token |
| 2 | `Login_Fails_WhenEmailIsEmpty` | Throws `ValidationException` |
| 3 | `Login_Fails_WhenPasswordIsEmpty` | Throws `ValidationException` |
| 4 | `Login_Fails_WhenUserNotFound` | Throws `UnauthorizedException` |
| 5 | `Login_Fails_WhenPasswordIsWrong` | Throws `UnauthorizedException` |
| 6 | `Login_CallsTokenService_ExactlyOnce` | `ITokenService.GenerateToken` called once |
| 7 | `Login_DoesNotCallTokenService_OnFailure` | `ITokenService.GenerateToken` never called when credentials are invalid |
| 8 | `Login_ReturnsToken_FromTokenService` | `LoginResult.Token` equals what `ITokenService` returned |

## Requirements

Use:

- xUnit
- Moq
- Shouldly

Place tests in: `backend/tests/TaskManager.UnitTests/Auth/`

Add a comment on each test explaining **why** it exists.

The tests must **not** pass until the production code is implemented in the next prompt.
