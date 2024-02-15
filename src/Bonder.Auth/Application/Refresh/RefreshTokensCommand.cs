using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Refresh;

public record RefreshTokensCommand(Tokens ExpiredTokens) : IRequest<Tokens>;