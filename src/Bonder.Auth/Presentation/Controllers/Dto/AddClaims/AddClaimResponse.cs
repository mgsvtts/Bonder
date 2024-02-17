namespace Presentation.Controllers.Dto.AddClaims;

public sealed record AddClaimResponse(string UserName, IEnumerable<UserClaim> Claims);