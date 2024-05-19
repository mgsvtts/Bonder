using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Trades;
using Mediator;

namespace Application.Commands.Operations.Create;

public sealed record CreateOperationCommand(PortfolioId PortfolioId,
                                            OperationName Name,
                                            string? Description,
                                            OperationType Type,
                                            OperationState State,
                                            DateTime Date,
                                            decimal Payout,
                                            decimal Price,
                                            decimal Commission,
                                            InstrumentType InstrumentType,
                                            Guid? InstrumentId,
                                            int Quantity,
                                            int RestQuantity,
                                            IEnumerable<Trade>? Trades) : IRequest;
