namespace Application.Commands.ImportPortfolio.Dto;

public readonly record struct ImportedOperation(DateOnly Date,
                                                TimeOnly Time,
                                                string Type,
                                                string Name,
                                                string Ticker,
                                                double Quantity,
                                                double Payout,
                                                double Price,
                                                double Commission);