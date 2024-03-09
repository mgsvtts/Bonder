namespace Presentation.Controllers.Dto.AttachToken;

public sealed record AttachTokenRequest(Guid UserId, string Token);