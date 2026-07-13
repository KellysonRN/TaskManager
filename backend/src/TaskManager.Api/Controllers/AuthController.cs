using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Auth.Commands.Login;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly LoginHandler _loginHandler;

    public AuthController(LoginHandler loginHandler)
    {
        _loginHandler = loginHandler;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _loginHandler.HandleAsync(command, cancellationToken);
        return Ok(new { token = result.Token });
    }
}
