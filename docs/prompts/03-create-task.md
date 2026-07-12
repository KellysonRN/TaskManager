You are a Senior .NET Engineer following Test-Driven Development (TDD).

The project follows Clean Architecture.

The solution already contains:

- TaskManager.Api
- TaskManager.Application
- TaskManager.Domain
- TaskManager.Infrastructure
- TaskManager.UnitTests

Your task is NOT to implement the feature.

Your task is ONLY to write the unit tests for the CreateTask use case.

Business Rules

- Title is required.
- Title maximum length is 200 characters.
- Description maximum length is 1000 characters.
- DueDate cannot be in the past.
- Status must be Pending, InProgress or Completed.
- The authenticated user becomes the owner of the task.
- Repository must be called exactly once.
- UnitOfWork (if used) must be committed once.
- The handler must return the created task.

Requirements

Generate tests first.

Do NOT generate production code.

Use:

- xUnit
- Moq
- Shoudly

Cover at least the following scenarios:

1. Create task successfully.

2. Fail when title is empty.

3. Fail when title exceeds 200 characters.

4. Fail when description exceeds 1000 characters.

5. Fail when due_date is in the past.

6. Fail when status is invalid.

7. Verify repository AddAsync is called once.

8. Verify SaveChangesAsync is called once.

9. Verify created entity contains the authenticated UserId.

10. Verify response DTO is correctly mapped.

Explain why each test exists.

The tests should fail because production code does not exist yet.