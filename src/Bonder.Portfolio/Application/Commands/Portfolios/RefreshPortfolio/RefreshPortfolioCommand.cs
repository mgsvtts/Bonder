using Domain.UserAggregate.ValueObjects.Users;
using Mediator;

namespace Application.Commands.Portfolios.RefreshPortfolio;
public sealed record RefreshPortfolioCommand(UserId UserId, TinkoffToken Token) : ICommand;