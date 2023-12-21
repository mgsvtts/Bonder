namespace Application.Calculation.CalculateTickers.Interfaces;

public interface ITInkoffHttpClient
{
    Task<decimal> GetBondPriceAsync(string bondId, CancellationToken token = default);
}