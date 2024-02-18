namespace Domain.UserAggregate.ValueObjects.Portfolios;

public record Portfolio(string Id, string Name, PortfolioType Type, PortfolioStatus Status);