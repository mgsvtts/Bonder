namespace Domain.UserAggregate.Repositories.Dto;

public readonly record struct OperationResult(bool IsSuccess, IEnumerable<string> ErrorMessages);