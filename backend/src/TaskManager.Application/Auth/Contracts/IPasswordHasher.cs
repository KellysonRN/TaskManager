namespace TaskManager.Application.Auth.Contracts;

public interface IPasswordHasher
{
    string Hash(string plain);
    bool Verify(string plain, string hash);
}
