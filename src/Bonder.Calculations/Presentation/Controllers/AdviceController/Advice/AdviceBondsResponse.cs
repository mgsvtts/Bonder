namespace Presentation.Controllers.AnalyzeController.Analyze;

public sealed record AdviceBondsResponse(string Ticker, decimal Income, IEnumerable<AnalyzeBondsResponseBond> BetterBonds);

public sealed record AnalyzeBondsResponseBond(string Ticker, string Name, decimal Price, decimal Income);