using Application.Commands.Analyze;

namespace Presentation.Controllers.AnalyzeController.Analyze;

public sealed record AnalyzeBondsRequest(AdviceOptions DefaultOptions, IEnumerable<BondToAdvice> Bonds);

public sealed record BondToAdvice(AdviceOptions Option, string Ticker);