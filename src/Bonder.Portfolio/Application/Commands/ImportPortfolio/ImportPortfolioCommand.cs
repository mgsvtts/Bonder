using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Mediator;

namespace Application.Commands.ImportPortfolio;
public sealed record ImportPortfolioCommand(UserId UserId, Stream FileStream, BrokerType BrokerType, string? Name) : ICommand;