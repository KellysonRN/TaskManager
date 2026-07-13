# 05 — Logout, Fixes & Implementation Notes

This prompt documents everything implemented beyond the original `04.x` series,
including the logout feature, infrastructure fixes, and deviations from the
planned approach that were required to make the system work end-to-end.

---

## Part A — Logout Feature

### Backend

No backend endpoint is required. Logout is handled entirely on the client by
discarding the JWT. The token is stateless — invalidation is implicit once it
expires.

### Frontend

**Logout button** — added to `list-tasks` header. Calls `AuthService.logout()`
which removes the token from `localStorage` and navigates to `/login`.

**Logout route** — `/logout` is a dedicated route backed by `LogoutComponent`:

```
frontend/src/app/src/features/auth/logout/logout.ts
```

Navigating to `/logout` triggers `AuthService.logout()` immediately via
`ngOnInit`. Useful for programmatic redirects (e.g. from email links or e2e
tests).

**Updated routes:**

```ts
{ path: 'login',  component: Login },
{ path: 'logout', component: Logout },           // ← new
{ path: 'tasks',  component: TaskList,  canActivate: [authGuard] },
{ path: 'tasks/create', component: CreateTask, canActivate: [authGuard] },
{ path: '',  redirectTo: 'login', pathMatch: 'full' },
{ path: '**', redirectTo: 'login' },
```

---

## Part B — Fixes & Deviations

### 1. `ITokenService` signature mismatch

The existing interface had `CreateToken(string subject, IEnumerable<string> roles)`
instead of the `GenerateToken(UserEntity user)` described in prompt `04`.
**Decision:** kept the existing signature and adapted `LoginHandler` to call
`CreateToken(user.Id.ToString(), Array.Empty<string>())`.

### 2. JWT secret minimum key size (HS256 requires ≥ 256 bits)

`appsettings.json` shipped with a 30-character secret (240 bits).
`Microsoft.IdentityModel.Tokens` enforces a minimum of 256 bits for HS256.
**Fix:** secret extended to 32 characters in `appsettings.json`.

```json
"Secret": "ReplaceWithStrongSecretKey123!XY"
```

> Replace this value with a cryptographically random 32+ character string before
> any real deployment.

### 3. Schema drift — `Users` table missing from existing `taskmanager.db`

`EnsureCreated()` does not apply schema changes to existing databases.
The existing `taskmanager.db` (created before `UserEntity` was added) was
missing the `Users` table.

**Fix in `Program.cs`:** after `EnsureCreated()`, probe the `Users` table. On
`SqliteException`, drop and recreate the database so the schema is always current.

```csharp
try
{
    dbContext.Database.EnsureCreated();
    _ = dbContext.Users.Any();          // probe — throws if table is absent
}
catch (SqliteException)
{
    dbContext.Database.EnsureDeleted(); // schema drift detected — recreate
    dbContext.Database.EnsureCreated();
}
```

> **Note:** this recreates the entire database (all data lost). Acceptable for
> a dev/demo project; use EF Core migrations in production.

### 4. Duplicate `JwtSettings` types

Two identical `JwtSettings` classes existed:
- `TaskManager.Api.Models.JwtSettings` — used by the JWT middleware
- `TaskManager.Application.Configuration.JwtSettings` — used by `JwtTokenService`

`Program.cs` only configured the `Api.Models` type in DI, so `JwtTokenService`
always received an empty secret.

**Fix:** configure both types from the same config section:

```csharp
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.Configure<TaskManager.Application.Configuration.JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
```

### 5. Wrong relative import paths (frontend)

The `src/app/src/` nesting is deeper than it looks. Three files had incorrect
`../` counts:

| File | Wrong | Correct |
|------|-------|---------|
| `core/services/auth.service.ts` → `environments/` | `../../../` | `../../../../` |
| `features/tasks/create-task/task.service.ts` → `environments/` | `../../../../` | `../../../../../` |
| `features/auth/login/login.ts` → `core/services/auth.service` | `../../core/` | `../../../core/` |

### 6. Integration test isolation — shared `WebApplicationFactory` collection

`IClassFixture<WebApplicationFactory<Program>>` created one factory per test
class. All classes shared the same `taskmanager.db` file, causing:
- Concurrent `EnsureDeleted` / `EnsureCreated` conflicts
- `UNIQUE constraint failed: Users.Email` from duplicate seed calls

**Fix:** single xUnit `[Collection("Integration")]` backed by one
`IntegrationTestFactory` (implements `IAsyncLifetime`). Schema creation and
seeding happen **once** before any test runs.

```csharp
[CollectionDefinition("Integration")]
public sealed class IntegrationCollection : ICollectionFixture<IntegrationTestFactory> { }
```

---

## Data Seed

| Field    | Value                   |
|----------|-------------------------|
| Email    | `admin@taskmanager.dev` |
| Password | `Admin@123`             |

Seeded automatically on first startup when the `Users` table is empty.
Skipped in the `Testing` environment (integration tests seed manually).
