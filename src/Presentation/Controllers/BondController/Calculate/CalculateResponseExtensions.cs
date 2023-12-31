using Application.Calculation.Common.CalculationService.Dto;
using Presentation.Controllers.BondController.Calculate.Response;

namespace Presentation.Controllers.BondController.Calculate;

public static class CalculateResponseExtensions
{
    public static CalculateResponse MapToResponse(this CalculationResults results)
    {
        return new CalculateResponse(CalculatedBonds: results.Results.Select(x => new CalculatedBondResponse(x.Bond.Id.Ticker.Value, x.Bond.Name, x.Priority)),
                                     PriceSortedBonds: results.PriceSortedBonds.Select(x => new PriceBondResponse(x.Bond.Id.Ticker.Value, x.Bond.Name, x.Money)),
                                     CouponIncomeSortedBonds: results.Bonds.Select(x => new IncomeBondResponse(x.Key.Id.Ticker.Value, x.Key.Name, x.Value.CouponIncome)).OrderByDescending(x => x.Income),
                                     NominalIncomeSortedBonds: results.Bonds.Select(x => new IncomeBondResponse(x.Key.Id.Ticker.Value, x.Key.Name, x.Value.NominalIncome)).OrderByDescending(x => x.Income),
                                     FullIncomeSortedBonds: results.FullIncomeSortedBonds.Select(x => new IncomeBondResponse(x.Bond.Id.Ticker.Value, x.Bond.Name, x.Money)),
                                     CreditRatingSortedBonds: results.RatingSortedBonds.Select(x => new CreditRatingBondResponse(x.Bond.Id.Ticker.Value, x.Bond.Name, x.Rating)));
    }
}