namespace Presentation.Controllers.BondController.Calculate.Response;

public sealed record PriceBondResponse(string Ticker, string Name, decimal Price);