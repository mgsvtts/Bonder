namespace Application.Calculation.Common.Interfaces;

public interface ITInkoffHttpClient
{
    Task<decimal> GetBondPriceAsync(string bondId, CancellationToken token = default);
}