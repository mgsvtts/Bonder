using Application.Commands.ImportPortfolio.Dto;
using Bonder.Calculation.Grpc;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Mapster;
using Mediator;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Text.RegularExpressions;

namespace Application.Commands.ImportPortfolio;

public sealed partial class ImportPortfolioCommandHandler : ICommandHandler<ImportPortfolioCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly CalculationService.CalculationServiceClient _grpcClient;

    public ImportPortfolioCommandHandler(CalculationService.CalculationServiceClient grpcClient, IUserRepository userRepository)
    {
        _grpcClient = grpcClient;
        _userRepository = userRepository;
    }

    public async ValueTask<Unit> Handle(ImportPortfolioCommand command, CancellationToken token)
    {
        var workbook = WorkbookFactory.Create(command.FileStream, true);

        var processingResult = await ProcessFileAsync(command, workbook.GetSheetAt(0), token);

        var bondsResult = await GetBondsAsync(processingResult, token);

        processingResult.User.AddImportedPortfolio
        (
            bondsResult.Bonds.Sum(x => x.Key.Price),
            command.BrokerType,
            bondsResult.Bonds.Adapt<IEnumerable<Bond>>(),
            (processingResult.Operations, bondsResult.OperationBonds).Adapt<IEnumerable<Operation>>(),
            command.Name
        );

        await _userRepository.SaveAsync(processingResult.User, token);

        return Unit.Value;
    }

    private async Task<GetBondsResult> GetBondsAsync(FileProcessingResult processingResult, CancellationToken token)
    {
        var countBondsTask = GetBondsAsync(processingResult.Tickers, token);
        var operationBondsTask = GetBondsAsync(processingResult.Operations.Select(x => x.Ticker).Distinct(), token);

        await Task.WhenAll(countBondsTask, operationBondsTask);

        var bonds = MergeBonds(countBondsTask.Result, processingResult.BoncCounts);

        return new GetBondsResult(operationBondsTask.Result, bonds);
    }

    private async Task<FileProcessingResult> ProcessFileAsync(ImportPortfolioCommand command, ISheet sheet, CancellationToken token)
    {
        var operationsTask = Task.Run(() => ProcessSheet(sheet, ParseOperation, Boundaries.OperationsRange1, Boundaries.OperationsRange2));
        var bondCountsTask = Task.Run(() => ProcessSheet(sheet, ParseBondCount, Boundaries.BondsCountRange1, Boundaries.BondsCountRange2));
        var tickersTask = Task.Run(() => ProcessSheet(sheet, ParseTickers, Boundaries.TickersRange1, Boundaries.TickersRange2));
        var userTask = _userRepository.GetByIdAsync(command.UserId, token);

        await Task.WhenAll(operationsTask, bondCountsTask, tickersTask, userTask);

        return new FileProcessingResult(operationsTask.Result, bondCountsTask.Result, tickersTask.Result, userTask.Result);
    }

    private static List<T> ProcessSheet<T>(ISheet sheet, Func<List<ICell>, T> func, string range1, string range2)
    {
        var range = GetRowsInRange(sheet, range1, range2);

        const int headerSize = 2;

        var result = new List<T>(range.LastRow - range.FirstRow);
        for (int i = range.FirstRow + headerSize; i < range.LastRow; i++)
        {
            var row = sheet.GetRow(i);

            if (row is null || ShouldSkip(row))
            {
                continue;
            }

            result.Add(func.Invoke(row.Cells));
        }

        return result;
    }

    private static ImportedOperation ParseOperation(List<ICell> cells)
    {
        var date = DateOnly.Parse(cells[3].StringCellValue);
        var time = TimeOnly.Parse(cells[4].StringCellValue);
        var type = cells[6].StringCellValue;
        var name = cells[7].StringCellValue;
        var ticker = cells[8].StringCellValue;
        var quantity = cells[11].NumericCellValue;
        var payout = cells[14].NumericCellValue;
        var price = payout / quantity;
        var commission = cells[16].NumericCellValue;

        return new ImportedOperation(date, time, type, name, ticker, quantity, payout, price, commission);
    }

    private static (string Ticker, double Count) ParseBondCount(List<ICell> cells)
    {
        return (cells[5].StringCellValue, cells[27].NumericCellValue);
    }

    private static string? ParseTickers(List<ICell> cells)
    {
        if (!cells[26].StringCellValue.Contains("обл", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return cells[6].StringCellValue;
    }

    private static CellRangeAddress GetRowsInRange(ISheet sheet, string range1, string range2)
    {
        var (startRow, endRow) = GetBoundaries(sheet, range1, range2);

        return CellRangeAddress.ValueOf($"{startRow.Cells[0].Address}:{endRow.Cells[^1].Address}");
    }

    private static (IRow Start, IRow End) GetBoundaries(ISheet sheet, string range1, string range2)
    {
        IRow startRow = null;
        IRow endRow = null;

        var enumerator = sheet.GetRowEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is not IRow row)
            {
                continue;
            }

            if (row.Cells.Any(x => x.CellType == CellType.String && x.StringCellValue.Equals(range1, StringComparison.OrdinalIgnoreCase)))
            {
                startRow = row;
            }

            if (row.Cells.Any(x => x.CellType == CellType.String && x.StringCellValue.Equals(range2, StringComparison.OrdinalIgnoreCase)))
            {
                endRow = row;
            }

            if (startRow is not null && endRow is not null)
            {
                break;
            }
        }

        if (startRow is null || endRow is null)
        {
            throw new InvalidOperationException("Cannot find file boundaries");
        }

        return (startRow, endRow);
    }

    private static bool ShouldSkip(IRow row)
    {
        var concated = string.Concat(row.Cells);

        return concated.Contains("из", StringComparison.OrdinalIgnoreCase) &&
               LastNumberRegex().IsMatch(concated) &&
               FirstNumberRegex().IsMatch(concated);
    }

    private async Task<IList<GrpcBond>> GetBondsAsync(IEnumerable<string?>? tickers, CancellationToken token)
    {
        if (tickers is null || !tickers.Any())
        {
            return [];
        }

        var request = new GetBondsByTickersRequest();
        request.Tickers.AddRange(tickers.Where(x => !string.IsNullOrEmpty(x)));

        var response = await _grpcClient.GetBondsByTickersAsync(request, cancellationToken: token);

        return response.Bonds;
    }

    private static Dictionary<GrpcBond, int> MergeBonds(IList<GrpcBond> bonds, IEnumerable<(string Ticker, double Count)> counts)
    {
        var result = new Dictionary<GrpcBond, int>();
        foreach (var (ticker, count) in counts)
        {
            var bond = bonds.FirstOrDefault(x => x.Ticker == ticker);
            if (bond is null)
            {
                continue;
            }

            result.Add(bond, (int)count);
        }

        return result;
    }

    [GeneratedRegex(@"\d+$")]
    private static partial Regex LastNumberRegex();

    [GeneratedRegex(@"^\d+")]
    private static partial Regex FirstNumberRegex();
}