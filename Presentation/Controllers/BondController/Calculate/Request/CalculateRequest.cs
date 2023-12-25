using Domain.BondAggreagte.Dto;

namespace Presentation.Controllers.BondController.Calculate.Request;
public sealed record CalculateRequest(GetIncomeRequest Options, IEnumerable<string> Tickers);