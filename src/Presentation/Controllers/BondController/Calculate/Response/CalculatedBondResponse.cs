namespace Presentation.Controllers.BondController.Calculate.Response;

public sealed record CalculatedBondResponse(string Ticker, string Name, int Priority);