using Application.Calculation.Common.CalculationService.Dto;
using Presentation.Controllers.BondController.Calculate.Response;

namespace Presentation.Controllers.BondController.Calculate;

public static class CalculateResponseExtensions
{
    public static CalculateResponse MapToResponse(this CalculationResults results)
    {
        return new CalculateResponse(CalculatedBonds: results.Results.Select(x => new CalculatedBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Priority)),
                                     PriceSortedBonds: results.PriceSortedBonds.Select(x => new PriceBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Money)),
                                     CouponIncomeSortedBonds: results.CouponIncomeSortedBonds.Select(x => new IncomeBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Money)),
                                     NominalIncomeSortedBonds: results.NominalIncomeSortedBonds.Select(x => new IncomeBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Money)),
                                     FullIncomeSortedBonds: results.FullIncomeSortedBonds.Select(x => new IncomeBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Money)));
    }
}