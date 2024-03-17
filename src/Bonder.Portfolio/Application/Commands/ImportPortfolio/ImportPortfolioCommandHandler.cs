using Application.Commands.ImportPortfolio.Dto;
using Bonder.Calculation.Grpc;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using IronXL;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Application.Commands.ImportPortfolio;

public sealed partial class ImportPortfolioCommandHandler : ICommandHandler<ImportPortfolioCommand>
{
    private const int _cellSize = 2;

    private const string _operationsRange1 = "1.1 Информация о совершенных и исполненных сделках на конец отчетного периода";
    private const string _operationsRange2 = "1.2 Информация о неисполненных сделках на конец отчетного периода";

    private const string _tickersRange1 = "4.1 Информация о ценных бумагах";
    private const string _tickersRange2 = "4.2 Информация об инструментах, не квалифицированных в качестве ценной бумаги";

    private const string _bondsCountRange1 = "3.1 Движение по ценным бумагам инвестора";
    private const string _bondsCountRange2 = "3.2 Движение по производным финансовым инструментам";

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

        var operationsTask = GetOperationsAsync(workbook, token);
        var bondsTask = GetBondsAsync(workbook, token);
        var userTask = _userRepository.GetByIdAsync(command.UserId, token);

        await Task.WhenAll(bondsTask, operationsTask, userTask);

        var bonds = bondsTask.Result;
        var operations = operationsTask.Result;
        var user = userTask.Result;

        user.AddImportedPortfolio(bonds.Sum(x => x.Key.Price), command.BrokerType, bonds.Adapt<IEnumerable<Bond>>(), operations, command.Name);

        await _userRepository.SaveAsync(user, token);

        return Unit.Value;
    }

    private async Task<Dictionary<GrpcBond, int>> GetBondsAsync(WorkBook workBook, CancellationToken token)
    {
        var counts = GetBondsCount(workBook);
        var bonds = await GetBondsAsync(GetTickers(workBook), token);

        return MergeBonds(counts, bonds);
    }

    private static Dictionary<GrpcBond, int> MergeBonds(Dictionary<string, int> counts, IList<GrpcBond> bonds)
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

    private static IEnumerable<string> GetTickers(WorkBook workBook)
    {
        var cells = GetBoundaredCells(workBook, _tickersRange1, _tickersRange2);
        for (var i = 0; i < cells.Count; i++)
        {
            var row = cells[i];
            if (ShouldSkip(row))
            {
                continue;
            }

            var ticker = ParseTicker(row.Columns.ToList());
            if (ticker is null)
            {
                continue;
            }

            yield return ticker;
        }
    }

    private async Task<IEnumerable<Operation>> GetOperationsAsync(WorkBook workBook, CancellationToken token)
    {
        var operations = new List<ImportedOperation>();

        var cells = GetBoundaredCells(workBook, _operationsRange1, _operationsRange2);
        for (var i = 0; i < cells.Count; i++)
        {
            var row = cells[i];
            if (ShouldSkip(row))
            {
                continue;
            }

            operations.Add(ParseOperation(row.Columns.ToList()));
        }

        var response = await GetBondsAsync(operations.Select(x => x.Ticker).Distinct(), token);

        return (operations, response).Adapt<IEnumerable<Operation>>();
    }

    private static Dictionary<string, int> GetBondsCount(WorkBook workBook)
    {
        var result = new Dictionary<string, int>();

        var cells = GetBoundaredCells(workBook, _bondsCountRange1, _bondsCountRange2);
        for (var i = 0; i < cells.Count; i++)
        {
            var row = cells[i];
            if (ShouldSkip(row))
            {
                continue;
            }

            var (ticker, count) = ParseBondCount(row.Columns.ToList());

            result.Add(ticker, count);
        }

        return result;
    }

    private static bool ShouldSkip(RangeRow row)
    {
        var trimmed = string.Concat(row
        .ToString()
        .Where(x => x != '\t' && x != '\n' && x != ' '));

        return trimmed.Contains("из", StringComparison.OrdinalIgnoreCase) &&
               LastNumberRegex().IsMatch(trimmed) && 
               FirstNumberRegex().IsMatch(trimmed);
    }

    private static List<RangeRow> GetBoundaredCells(WorkBook workBook, string startBoundary, string endBoundary)
    {
        var start = workBook.DefaultWorkSheet.FirstOrDefault(x => x.Text.Equals(startBoundary, StringComparison.OrdinalIgnoreCase));
        var end = workBook.DefaultWorkSheet.FirstOrDefault(x => x.Text.Equals(endBoundary, StringComparison.OrdinalIgnoreCase));

        if (start is null || end is null)
        {
            throw new InvalidOperationException("Cannot find file boundaries");
        }

        return workBook
        .DefaultWorkSheet[$"{start.Address.FirstRow + _cellSize}:AF{end.Address.LastRow}"]
        .Rows
        .Where(x => !x.IsEmpty)
        .Skip(1)
        .ToList();
    }

    private static ImportedOperation ParseOperation(List<RangeColumn> columns)
    {
        var date = DateOnly.Parse(columns[3].StringValue);
        var time = TimeOnly.Parse(columns[4].StringValue);
        var type = columns[6].StringValue;
        var name = columns[7].StringValue;
        var ticker = columns[8].StringValue;
        var quantity = int.Parse(columns[11].StringValue);
        var payout = decimal.Parse(columns[14].StringValue);
        var price = payout / quantity;
        var commission = decimal.Parse(columns[16].StringValue);

        return new ImportedOperation(date, time, type, name, ticker, quantity, payout, price, commission);
    }

    private static string? ParseTicker(List<RangeColumn> columns)
    {
        if (!columns[26].StringValue.Contains("обл", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return columns[6].StringValue;
    }

    private static (string Ticker, int Count) ParseBondCount(List<RangeColumn> columns)
    {
        return (columns[5].StringValue, columns[27].Int32Value);
    }

    private async Task<IList<GrpcBond>> GetBondsAsync(IEnumerable<string> tickers, CancellationToken token)
    {
        var request = new GetBondsByTickersRequest();
        request.Tickers.AddRange(tickers);

        var response = await _grpcClient.GetBondsByTickersAsync(request, cancellationToken: token);

        return response.Bonds;
    }

    [GeneratedRegex(@"\d+$")]
    private static partial Regex LastNumberRegex();
    [GeneratedRegex(@"^\d+")]
    private static partial Regex FirstNumberRegex();
}