using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Mediator;

namespace Application.ImportPortfolio;
public sealed record ImportPortfolioCommand(UserId UserId, Stream FileStream, BrokerType BrokerType) : ICommand;