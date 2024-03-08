using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.ValueObjects.Identities;
using Domain.BondAggreagte.ValueObjects.Incomes;
using Domain.BondAggreagte.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.BondAggreagte.Abstractions.Dto;
using Application.Calculation.Common.CalculationService.Dto;

namespace Infrastructure.Common.JsonConverters;
public sealed class CalculateAllResponseConverter : JsonConverter<CalculateAllResponse>
{
    public override CalculateAllResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);

        var currentPage = int.Parse(node["PageInfo"]["CurrentPage"].ToString());
        var lastPage = int.Parse(node["PageInfo"]["LastPage"].ToString());
        var itemsOnPage = int.Parse(node["PageInfo"]["ItemsOnPage"].ToString());
        var total = int.Parse(node["PageInfo"]["Total"].ToString());

        var prioritySortedBonds = node["Aggregation"]["PrioritySortedBonds"].Deserialize<List<CalculationResult>>(options);
        var fullIncomeSortedBonds = node["Aggregation"]["FullIncomeSortedBonds"].Deserialize<List<CalculationMoneyResult>>(options);
        var bondsWithIncome = node["Aggregation"]["BondsWithIncome"].Deserialize<List<BondWithIncome>>(options);
        var priceSortedBonds = node["Aggregation"]["PriceSortedBonds"].Deserialize<List<CalculationMoneyResult>>(options);

        var calculationResultsConstructor = typeof(CalculationResults)
        .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
        .First();

        var calculationResults = (CalculationResults)calculationResultsConstructor.Invoke(
        [
            prioritySortedBonds,
            bondsWithIncome,
            priceSortedBonds,
            fullIncomeSortedBonds
        ]);

        return new CalculateAllResponse(calculationResults, new PageInfo(currentPage, lastPage, itemsOnPage, total));
    }

    public override void Write(Utf8JsonWriter writer, CalculateAllResponse value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}