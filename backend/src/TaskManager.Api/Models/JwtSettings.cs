namespace TaskManager.Api.Models;

/// <summary>
/// Configuration settings for JWT authentication.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    /// Gets or sets the issuer of the JWT tokens.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the intended audience for the JWT tokens.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the signing secret used to generate JWT tokens.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiry time in minutes.
    /// </summary>
    public int ExpiryMinutes { get; set; }
}
