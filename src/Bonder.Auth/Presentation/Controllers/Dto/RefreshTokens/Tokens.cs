using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Dto.RefreshTokens;
public sealed record Tokens([FromHeader(Name = "X-REFRESH-TOKEN")] string? RefreshToken,
                            [FromHeader(Name = "X-ACCESS-TOKEN")] string? AccessToken);