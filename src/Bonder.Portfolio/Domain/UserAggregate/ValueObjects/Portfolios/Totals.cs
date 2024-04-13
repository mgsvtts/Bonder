using System.Text.Json.Serialization;

namespace Domain.UserAggregate.ValueObjects.Portfolios;

public readonly record struct Totals(decimal TotalBondPrice,
                                     decimal TotalSharePrice,
                                     decimal TotalEtfPrice,
                                     decimal TotalCurrencyPrice,
                                     decimal TotalFuturePrice,
                                     decimal TotalPortfolioPrice);