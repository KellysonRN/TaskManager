namespace TaskManager.Application.Common.Cqrs;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
