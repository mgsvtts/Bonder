using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AttachTinkoffToken;
public sealed record AttachTinkoffTokenCommand(UserName UserName, string Token) : IRequest<User>;
