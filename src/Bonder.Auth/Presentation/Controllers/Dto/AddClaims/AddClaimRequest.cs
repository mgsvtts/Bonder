using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Dto.AddClaims;

public sealed record AddClaimRequest([FromHeader(Name = "X-USER-ID")] Guid CurrentUserId, [FromBody] string AddTo, [FromBody] IEnumerable<UserClaim> Claims);