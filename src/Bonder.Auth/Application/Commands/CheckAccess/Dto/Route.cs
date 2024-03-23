namespace Application.Commands.CheckAccess.Dto;

public readonly record struct Route(bool Authorized, IEnumerable<string> Claims);
