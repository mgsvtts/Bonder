using Application.Commands.ImportPortfolio.Dto;
using Bonder.Calculation.Grpc;
using Domain.Common.Abstractions;
using Domain.Common.Abstractions.Dto;
using Domain.UserAggregate;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Mapster;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Commands.ImportPortfolio.Common;
public sealed partial class PortfolioImporter : IPortfolioImporter
{
    private readonly CalculationService.CalculationServiceClient _grpcClient;

    public PortfolioImporter(CalculationService.CalculationServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<Portfolio> ImportAsync(AddPortfoliosToUserRequest request, CancellationToken token)
    {
        var workbooks = GetWorkbooks(request.Streams);

        var operationsTask = ProcessOperationsAsync(workbooks);
        var countsTask = Task.Run(() => ProcessBondsCount(workbooks));

        await Task.WhenAll(countsTask, operationsTask);

        var bondsResult = await GetBondsAsync(countsTask.Result, operationsTask.Result, token);

        return new Portfolio
        (
            new PortfolioId(Guid.NewGuid()),
            new Totals(bondsResult.Bonds.Sum(x => x.Key.Price), 0, 0, 0, 0, 0),
            request.Name ?? $"{request.BrokerType}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}",
            PortfolioType.Exported,
            request.BrokerType,
            bondsResult.Bonds.Adapt<IEnumerable<Bond>>(),
            (operationsTask.Result, bondsResult.OperationBonds).Adapt<IEnumerable<Operation>>()
        );
    }

    private static async Task<IEnumerable<ImportedOperation>> ProcessOperationsAsync(IEnumerable<IWorkbook> workbooks)
    {
        var result = new List<Task<List<ImportedOperation>>>();

        foreach (var workbook in workbooks)
        {
            var task = Task.Run(() => ProcessSheet(workbook.GetSheetAt(0), ParseOperation, Boundaries.OperationsRange1, Boundaries.OperationsRange2));

            result.Add(task);
        }

        await Task.WhenAll(result);

        return result.SelectMany(x => x.Result).Distinct();
    }

    private static List<IWorkbook> GetWorkbooks(IEnumerable<Stream> streams)
    {
        return streams.Select(x => WorkbookFactory.Create(x, true))
        .ToList();
    }

    private static Dictionary<string, int> ProcessBondsCount(IEnumerable<IWorkbook> workbooks)
    {
        var newestWorkbook = GetNewestWorkbook(workbooks);

        return ProcessSheet(newestWorkbook.GetSheetAt(0), ParseBondCount, Boundaries.BondsCountRange1, Boundaries.BondsCountRange2)
        .ToDictionary();
    }

    private static IWorkbook GetNewestWorkbook(IEnumerable<IWorkbook> workbooks)
    {
        (IWorkbook newestWorkbook, DateOnly newestDate) = (workbooks.First(), DateOnly.MinValue);

        foreach (var workbook in workbooks)
        {
            var dates = ProcessSheet(workbook.GetSheetAt(0), ParseFileDate, range: CellRangeAddress.ValueOf("A1:AH7"));
            var date = dates.FirstOrDefault(x => x.HasValue);

            if (date > newestDate)
            {
                newestWorkbook = workbook;
                newestDate = date.Value;
            }
        }

        return newestWorkbook;
    }

    private async Task<GetBondsResult> GetBondsAsync(Dictionary<string, int> counts, IEnumerable<ImportedOperation> operations, CancellationToken token)
    {
        var countBondsTask = GetBondsAsync(counts.Keys, token);
        var operationBondsTask = GetBondsAsync(operations.Select(x => x.Ticker).Distinct(), token);

        await Task.WhenAll(countBondsTask, operationBondsTask);

        var bonds = MergeBonds(countBondsTask.Result, counts);

        return new GetBondsResult(operationBondsTask.Result, bonds);
    }

    private static List<T> ProcessSheet<T>(ISheet sheet, Func<List<ICell>, T> func, string? range1 = null, string? range2 = null, CellRangeAddress? range = null)
    {
        range ??= GetRowsInRange(sheet, range1, range2);

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
        var date = DateOnly.ParseExact(cells[3].StringCellValue, "dd.MM.yyyy");
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

    private static KeyValuePair<string, int> ParseBondCount(List<ICell> cells)
    {
        return new(cells[5].StringCellValue, (int)cells[27].NumericCellValue);
    }

    private static DateOnly? ParseFileDate(List<ICell> cells)
    {
        var match = FileDateRegex().Match(cells[0].StringCellValue);

        if (!match.Success)
        {
            return null;
        }

        return DateOnly.ParseExact(match.Groups[1].Value, "dd.MM.yyyy");
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

    private static Dictionary<GrpcBond, int> MergeBonds(IList<GrpcBond> bonds, Dictionary<string, int> counts)
    {
        var result = new Dictionary<GrpcBond, int>();
        foreach (var (ticker, count) in counts)
        {
            var bond = bonds.FirstOrDefault(x => x.Ticker == ticker);
            if (bond is null)
            {
                continue;
            }

            result.Add(bond, count);
        }

        return result;
    }

    [GeneratedRegex(@"\d+$")]
    private static partial Regex LastNumberRegex();

    [GeneratedRegex(@"^\d+")]
    private static partial Regex FirstNumberRegex();

    [GeneratedRegex(@"(?<=Дата\sрасчета:\s)(\d{2}.\d{2}.\d{4})")]
    private static partial Regex FileDateRegex();
}
