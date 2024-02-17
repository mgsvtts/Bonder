namespace Presentation.Controllers.BondController.Calculate.Response;

public readonly record struct PriceBondResponse(string Ticker, string Name, decimal Price);