namespace Disputes.Api.Domain.Entities
{
    public sealed class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Role { get; set; } = "Customer";
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
