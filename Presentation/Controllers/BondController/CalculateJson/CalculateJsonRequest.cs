namespace Presentation.Controllers.BondController.CalculateJson;

public sealed record CalculateJsonRequest(IEnumerable<Bond> Bonds);

public sealed record Bond(string Ticker,
                          string Name,
                          decimal Price,
                          DateTime EndDate,
                          decimal Denomination,
                          Coupon Coupon);

public sealed record Coupon(DateTime Date,
                            decimal Payout,
                            int PayRate);