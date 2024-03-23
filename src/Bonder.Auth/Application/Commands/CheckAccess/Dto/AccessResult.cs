using Domain.UserAggregate.ValueObjects;

namespace Application.Commands.CheckAccess.Dto;
public readonly record struct AccessResult(bool AccessAllowed, bool TokenExpired, UserId? UserId)
{
    public static AccessResult NotAllowed(bool tokenExpired = false)
    {
        return new AccessResult(false, tokenExpired, null);
    }

    public static AccessResult Allowed(UserId? userId, bool tokenExpired = false)
    {
        return new AccessResult(true, false, userId);
    }
}
