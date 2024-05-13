using Domain.Common.Abstractions;
using Domain.Common.Abstractions.Dto;
using Domain.UserAggregate.Abstractions.Repositories;
using Mapster;
using Mediator;

namespace Application.Commands.Portfolios.ImportPortfolio;

public sealed partial class ImportPortfolioCommandHandler : ICommandHandler<ImportPortfolioCommand>
{
    private readonly IPortfolioImporter _importer;
    private readonly IUserRepository _userRepository;

    public ImportPortfolioCommandHandler(IPortfolioImporter importer, IUserRepository userRepository)
    {
        _importer = importer;
        _userRepository = userRepository;
    }

    public async ValueTask<Unit> Handle(ImportPortfolioCommand command, CancellationToken token)
    {
        var userTask = _userRepository.GetOrCreateByIdAsync(command.UserId, token);

        var portfolioTask = _importer.ImportAsync(command.Adapt<AddPortfoliosToUserRequest>(), token);

        await Task.WhenAll(userTask, portfolioTask);

        userTask.Result.AddPortfolio(portfolioTask.Result);

        await _userRepository.SaveAsync(userTask.Result, token);

        return Unit.Value;
    }
}