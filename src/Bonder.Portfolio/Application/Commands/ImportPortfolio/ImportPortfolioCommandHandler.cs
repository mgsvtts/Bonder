using Bonder.Calculation.Grpc;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects.Portfolios;
using IronXL;
using Mapster;
using Mediator;

namespace Application.Commands.ImportPortfolio;

public sealed class ImportPortfolioCommandHandler : ICommandHandler<ImportPortfolioCommand>
{
    private static readonly int _cellSize = 2;
    private static readonly int _headerSize = 32;

    private static readonly List<string> _excludeWords =
    [
        "4 из ",
        "6",
        "Сокращенное \nнаименование актива",
        "Код актива",
        "Код государственной регистрации",
        "Наименование эмитента",
        "Тип"
    ];

    private readonly CalculationService.CalculationServiceClient _grpcClient;
    private readonly IUserRepository _userRepository;

    public ImportPortfolioCommandHandler(CalculationService.CalculationServiceClient grpcClient, IUserRepository userRepository)
    {
        _grpcClient = grpcClient;
        _userRepository = userRepository;
    }

    public async ValueTask<Unit> Handle(ImportPortfolioCommand command, CancellationToken token)
    {
        var workbook = WorkBook.Load(command.FileStream);

        var cells = GetCellsInRange(workbook.DefaultWorkSheet);
        var tickers = GetTickers(cells);

        var bondsTask = GetBondsAsync(tickers, token);
        var userTask = _userRepository.GetByIdAsync(command.UserId, token);

        await Task.WhenAll(bondsTask, userTask);

        var user = userTask.Result;
        var bonds = bondsTask.Result;

        user.AddImportedPortfolio(bonds.Sum(x => x.Price), command.BrokerType, bonds.Adapt<IEnumerable<Bond>>(), command.Name);

        await _userRepository.SaveAsync(user, token);

        return Unit.Value;
    }

    private async Task<IList<GrpcBond>> GetBondsAsync(List<string> tickers, CancellationToken token)
    {
        var request = new GetBondsByTickersRequest();
        request.Tickers.AddRange(tickers);

        var response = await _grpcClient.GetBondsByTickersAsync(request, cancellationToken: token);

        return response.Bonds;
    }

    private static List<string> GetTickers(List<Cell> cells)
    {
        var tickers = new List<string>();
        for (int i = 0; i < cells.Count; i += 5)
        {
            if (NotBond(cells, i))
            {
                continue;
            }

            tickers.Add(cells[i + 1].Text);
        }

        return tickers;
    }

    private static bool NotBond(List<Cell> cells, int i)
    {
        return !cells[i + 4].Text.Contains("обл");
    }

    private static List<Cell> GetCellsInRange(WorkSheet worksheet)
    {
        var startCell = worksheet[worksheet.RangeAddress.Location]
        .FirstOrDefault(x => x.Text.Equals("4.1 Информация о ценных бумагах", StringComparison.OrdinalIgnoreCase));

        var endCell = worksheet[worksheet.RangeAddress.Location]
        .FirstOrDefault(x => x.Text.Equals("4.2 Информация об инструментах, не квалифицированных в качестве ценной бумаги", StringComparison.OrdinalIgnoreCase));

        if (startCell is null || endCell is null)
        {
            throw new InvalidOperationException("Cannot find file boundaries");
        }

        return worksheet[$"{startCell.Address.FirstRow + _cellSize}:AF{endCell.Address.LastRow}"]
        .Skip(_headerSize)
        .Where(x => x.Value is not "" && !_excludeWords.Any(y => y == x.Text))
        .ToList();
    }
}