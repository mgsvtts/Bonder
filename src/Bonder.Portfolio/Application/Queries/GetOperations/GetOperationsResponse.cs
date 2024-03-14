using Domain.UserAggregate.ValueObjects.Operations;
using Shared.Domain.Common;

namespace Application.Queries.GetOperations;

public readonly record struct GetOperationsResponse(IEnumerable<Operation> Operations, PageInfo PageInfo);