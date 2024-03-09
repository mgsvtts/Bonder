using Bonder.Calculation.Grpc;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects.Portfolios;
using IronXL;
using Mapster;
using Mediator;

namespace Application.ImportPortfolio;

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

    public async ValueTask<Unit> Handle(ImportPortfolioCommand command, CancellationToken cancellationToken)
    {
        var workbook = WorkBook.Load(command.FileStream);

        var cells = GetCellsInRange(workbook.DefaultWorkSheet);
        var tickers = GetTickers(cells);
        var bonds = await GetBondsAsync(tickers, cancellationToken);

        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

        user.AddPortfolio(bonds.Sum(x => x.Price), command.BrokerType, bonds.Adapt<IEnumerable<Bond>>());

        await _userRepository.SaveAsync(user, cancellationToken);

        return Unit.Value;
    }

    private async Task<IList<GrpcBond>> GetBondsAsync(List<string> tickers, CancellationToken cancellationToken)
    {
        var request = new GetBondsByTickersRequest();
        request.Tickers.AddRange(tickers);

        var response = await _grpcClient.GetBondsByTickersAsync(request, cancellationToken: cancellationToken);

        return response.Bonds;
    }

    private static List<string> GetTickers(List<Cell> cells)
    {
        var tickers = new List<string>();
        for (int i = 0; i < cells.Count; i += 5)
        {
            if (!cells[i + 4].Text.Contains("обл"))
            {
                continue;
            }

            tickers.Add(cells[i + 1].Text);
        }

        return tickers;
    }

    private static List<Cell> GetCellsInRange(WorkSheet worksheet)
    {
        var startCell = worksheet[worksheet.RangeAddress.Location].FirstOrDefault(x => x.Text.Equals("4.1 Информация о ценных бумагах", StringComparison.OrdinalIgnoreCase));
        var endCell = worksheet[worksheet.RangeAddress.Location].FirstOrDefault(x => x.Text.Equals("4.2 Информация об инструментах, не квалифицированных в качестве ценной бумаги", StringComparison.OrdinalIgnoreCase));

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