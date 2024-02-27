namespace Domain.UserAggregate.ValueObjects.Portfolios;

public readonly record struct Portfolio(decimal TotalBondPrice, string Name, PortfolioType Type, PortfolioStatus Status, IEnumerable<Bond> Bonds);
