namespace Presentation.Controllers.BondController.Calculate.Response;

public readonly record struct CalculatedBondResponse(string Ticker, string Name, int Priority);