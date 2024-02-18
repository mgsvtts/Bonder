namespace Presentation.Controllers.Dto.RemoveClaims;
public sealed record RemoveClaimRequest(string UserName, IEnumerable<string> Claims);