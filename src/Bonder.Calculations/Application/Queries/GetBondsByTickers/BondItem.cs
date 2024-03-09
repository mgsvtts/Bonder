namespace Application.Queries.GetBondsByTickers;

public sealed record BondItem(Guid Id,
                              string Ticker,
                              string Isin,
                              string Name,
                              decimal Price,
                              decimal Nominal,
                              DateOnly? MaturityDate,
                              DateOnly? OfferDate,
                              int? Rating);