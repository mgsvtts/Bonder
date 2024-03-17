namespace Application.Commands.ImportPortfolio.Dto;

public readonly record struct ImportedOperation(DateOnly Date,
                                                TimeOnly Time,
                                                string Type,
                                                string Name,
                                                string Ticker,
                                                int Quantity,
                                                decimal Payout,
                                                decimal Price,
                                                decimal Commission);

