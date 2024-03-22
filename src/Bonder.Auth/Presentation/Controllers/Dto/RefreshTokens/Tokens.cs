using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers.Dto.RefreshTokens;
public sealed record Tokens([FromHeader(Name = "X-REFRESH-TOKEN")] string? RefreshToken,
                            [FromHeader(Name = "X-ACCESS-TOKEN")] string? AccessToken);