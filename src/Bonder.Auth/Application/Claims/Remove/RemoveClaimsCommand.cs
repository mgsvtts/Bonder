using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Claims.Remove;
public sealed record RemoveClaimsCommand(UserName RequestedBy, UserName AddTo, IEnumerable<string> Claims) : IRequest<User>;
