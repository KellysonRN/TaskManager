namespace TaskManager.Application.Contracts;

public interface ITokenService
{
    string CreateToken(string subject, IEnumerable<string> roles);
}
