using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Login;

public record LoginCommand(string UserName, string Password) : IRequest<Tokens>;