using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Abstractions.Dto;
public sealed record AddPortfoliosToUserRequest(BrokerType BrokerType, string? Name, IEnumerable<Stream> Streams);