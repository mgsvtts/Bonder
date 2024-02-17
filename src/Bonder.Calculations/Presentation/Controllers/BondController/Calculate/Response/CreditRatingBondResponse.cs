namespace Presentation.Controllers.BondController.Calculate.Response;
public readonly record struct CreditRatingBondResponse(int? Rating, IEnumerable<CreditRatingBond> Bonds);

public readonly record struct CreditRatingBond(string Ticker, string Name);