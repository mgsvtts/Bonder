using Application.Calculation.CalculateTickers;

namespace Presentation.Controllers.BondController.Calculate.Request;
public sealed record CalculateBondsRequest(CalculationOptions Options, IdType IdType, IEnumerable<string> Ids);