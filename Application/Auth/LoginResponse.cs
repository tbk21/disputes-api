namespace Disputes.Api.Application.Auth;

public sealed record LoginResponse(
    string AccessToken,
    UserDto User
);

public sealed record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string Role
);