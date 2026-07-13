using Moq;
using Shouldly;
using Xunit;
using TaskManager.Application.Auth.Commands.Login;
using TaskManager.Application.Auth.Contracts;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Contracts;
using TaskManager.Domain;

namespace TaskManager.UnitTests.Auth;

public class LoginHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    private LoginHandler CreateHandler() =>
        new(_userRepositoryMock.Object, _passwordHasherMock.Object, _tokenServiceMock.Object);

    private static UserEntity FakeUser(string email = "user@test.com", string hash = "hashed") =>
        new() { Id = Guid.NewGuid(), Email = email, PasswordHash = hash };

    // Test 1: Happy path — valid credentials return a token
    [Fact]
    public async Task Login_Success()
    {
        var user = FakeUser();
        _userRepositoryMock.Setup(r => r.FindByEmailAsync("user@test.com", default)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("pass", user.PasswordHash)).Returns(true);
        _tokenServiceMock.Setup(t => t.CreateToken(user.Id.ToString(), It.IsAny<IEnumerable<string>>())).Returns("jwt-token");

        var result = await CreateHandler().HandleAsync(new LoginCommand { Email = "user@test.com", Password = "pass" });

        result.ShouldNotBeNull();
        result.Token.ShouldNotBeNullOrWhiteSpace();
    }

    // Test 2: Email is mandatory — omitting it must fail validation before touching the DB
    [Fact]
    public async Task Login_Fails_WhenEmailIsEmpty()
    {
        await Should.ThrowAsync<ValidationException>(() =>
            CreateHandler().HandleAsync(new LoginCommand { Email = "", Password = "pass" }));
    }

    // Test 3: Password is mandatory — omitting it must fail validation before touching the DB
    [Fact]
    public async Task Login_Fails_WhenPasswordIsEmpty()
    {
        await Should.ThrowAsync<ValidationException>(() =>
            CreateHandler().HandleAsync(new LoginCommand { Email = "user@test.com", Password = "" }));
    }

    // Test 4: Unknown email must not reveal whether the account exists
    [Fact]
    public async Task Login_Fails_WhenUserNotFound()
    {
        _userRepositoryMock.Setup(r => r.FindByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync((UserEntity?)null);

        await Should.ThrowAsync<UnauthorizedException>(() =>
            CreateHandler().HandleAsync(new LoginCommand { Email = "ghost@test.com", Password = "pass" }));
    }

    // Test 5: Wrong password must not reveal which part of the credentials is wrong
    [Fact]
    public async Task Login_Fails_WhenPasswordIsWrong()
    {
        var user = FakeUser();
        _userRepositoryMock.Setup(r => r.FindByEmailAsync("user@test.com", default)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("wrong", user.PasswordHash)).Returns(false);

        await Should.ThrowAsync<UnauthorizedException>(() =>
            CreateHandler().HandleAsync(new LoginCommand { Email = "user@test.com", Password = "wrong" }));
    }

    // Test 6: Exactly one token must be issued per successful login
    [Fact]
    public async Task Login_CallsTokenService_ExactlyOnce()
    {
        var user = FakeUser();
        _userRepositoryMock.Setup(r => r.FindByEmailAsync("user@test.com", default)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("pass", user.PasswordHash)).Returns(true);
        _tokenServiceMock.Setup(t => t.CreateToken(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns("t");

        await CreateHandler().HandleAsync(new LoginCommand { Email = "user@test.com", Password = "pass" });

        _tokenServiceMock.Verify(t => t.CreateToken(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Once);
    }

    // Test 7: No token must be generated when authentication fails
    [Fact]
    public async Task Login_DoesNotCallTokenService_OnFailure()
    {
        _userRepositoryMock.Setup(r => r.FindByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync((UserEntity?)null);

        await Should.ThrowAsync<UnauthorizedException>(() =>
            CreateHandler().HandleAsync(new LoginCommand { Email = "x@x.com", Password = "p" }));

        _tokenServiceMock.Verify(t => t.CreateToken(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    // Test 8: The token in LoginResult must come from ITokenService, not a hardcoded value
    [Fact]
    public async Task Login_ReturnsToken_FromTokenService()
    {
        var user = FakeUser();
        const string expectedToken = "service-generated-token";
        _userRepositoryMock.Setup(r => r.FindByEmailAsync("user@test.com", default)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("pass", user.PasswordHash)).Returns(true);
        _tokenServiceMock.Setup(t => t.CreateToken(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns(expectedToken);

        var result = await CreateHandler().HandleAsync(new LoginCommand { Email = "user@test.com", Password = "pass" });

        result.Token.ShouldBe(expectedToken);
    }
}
