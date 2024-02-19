using System.Threading.Channels;

namespace Domain.UserAggregate.ValueObjects.Portfolios;

public readonly record struct Portfolio(string Id, decimal TotalBondPrice, string Name, PortfolioType Type, PortfolioStatus Status, IEnumerable<Ticker> Tickers);

public readonly record struct Ticker(string Value);