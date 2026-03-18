namespace Disputes.Api.Application.Auth;

public sealed record LoginRequest(string Email, string Password);