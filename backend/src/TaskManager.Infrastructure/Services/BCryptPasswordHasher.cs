using TaskManager.Application.Auth.Contracts;

namespace TaskManager.Infrastructure.Services;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plain) => BCrypt.Net.BCrypt.HashPassword(plain);

    public bool Verify(string plain, string hash) => BCrypt.Net.BCrypt.Verify(plain, hash);
}
