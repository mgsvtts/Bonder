using Domain.UserAggregate.ValueObjects.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers.Operations.Dto.Create;

public sealed record CreateOperationRequest(Guid PortfolioId,
                                            string Name,
                                            string Description,
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
                                            IEnumerable<CreateOperationRequestTrade>? Trades = null);

public sealed record CreateOperationRequestTrade(DateTime Date,
                                                 int Quantity,
                                                 decimal Price);