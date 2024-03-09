namespace Domain.UserAggregate.ValueObjects.Portfolios;

public readonly record struct Portfolio(decimal TotalBondPrice, string Name, PortfolioType Type, BrokerType Broker, IEnumerable<Bond> Bonds);