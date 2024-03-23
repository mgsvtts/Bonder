using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Dto.CheckAccess;

public sealed record CheckAccessRequest([FromBody] string Path, [FromHeader(Name = "X-ACCESS-TOKEN")] string AccessToken);