using Domain.UserAggregate;

namespace Application.Commands.ImportPortfolio.Dto;

public readonly record struct FileProcessingResult(List<ImportedOperation> Operations,
                                                   List<(string Ticker, double Count)> BoncCounts,
                                                   List<string?> Tickers,
                                                   User User);