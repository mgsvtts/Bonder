using Application.Commands.Analyze;

namespace Presentation.Controllers.AnalyzeController.Analyze;

public sealed record AnalyzeBondsRequest(AnalyzeOptions DefaultOptions, IEnumerable<BondToAnalyze> Bonds);

public sealed record BondToAnalyze(AnalyzeOptions Option, string Ticker);