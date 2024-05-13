using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Trades;
using Mediator;
using Shared.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Operations.Create;

public sealed record CreateOperationCommand(PortfolioId PortfolioId,
                                            ValidatedString Name,
                                            ValidatedString? Description,
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
