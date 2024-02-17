namespace Presentation.Controllers.Dto.AddClaims;

public sealed record AddClaimRequest(string UserName, IEnumerable<UserClaim> Claims);