namespace Shared.Contracts;

public sealed class UserDeletedEvent
{
    public Guid UserId { get; set; }
}