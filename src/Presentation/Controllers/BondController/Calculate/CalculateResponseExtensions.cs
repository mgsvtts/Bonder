using Application.Calculation.Common.CalculationService;
using Presentation.Controllers.BondController.Calculate.Response;

namespace Presentation.Controllers.BondController.Calculate;

public static class CalculateResponseExtensions
{
    public static CalculateResponse MapToResponse(this CalculationResults results)
    {
        return new CalculateResponse(results.Results.Select(x => new CalculatedBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Priority)),
                                     results.Results.Select(x => new PriceBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Price)),
                                     results.Results.Select(x => new CouponeIncomeBondResponse(x.Bond.Id.Value, x.Bond.Name, x.CouponIncome)),
                                     results.Results.Select(x => new IncomeBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Income)));
    }
}