namespace Presentation.Controllers.AdviceController.Advice;

public sealed record AdviceBondsResponse(string Ticker, decimal Income, IEnumerable<AnalyzeBondsResponseBond> BetterBonds);

public sealed record AnalyzeBondsResponseBond(string Ticker, string Name, decimal Price, decimal Income);