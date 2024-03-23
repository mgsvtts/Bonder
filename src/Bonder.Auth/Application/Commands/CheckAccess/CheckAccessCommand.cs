using Application.Commands.CheckAccess.Dto;
using Mediator;

namespace Application.Commands.CheckAccess;
public sealed record CheckAccessCommand(string Path, string? AccessToken) : ICommand<AccessResult>;
