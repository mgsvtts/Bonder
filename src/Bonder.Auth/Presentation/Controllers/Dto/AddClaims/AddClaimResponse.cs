namespace Presentation.Controllers.Dto.AddClaims;

public record AddClaimResponse(string UserName, IEnumerable<UserClaim> Claims);