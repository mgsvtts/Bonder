namespace Presentation.Controllers.BondController.Calculate.Request;
public sealed record CalculateRequest(CalculationOptions Options, IEnumerable<string> Tickers);
