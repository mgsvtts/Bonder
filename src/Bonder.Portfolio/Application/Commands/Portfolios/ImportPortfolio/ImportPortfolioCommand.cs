using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Mediator;

namespace Application.Commands.Portfolios.ImportPortfolio;
public sealed record ImportPortfolioCommand(UserId UserId, BrokerType BrokerType, string? Name, IEnumerable<Stream> Streams) : ICommand;
