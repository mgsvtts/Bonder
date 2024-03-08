using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Incomes;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Infrastructure.Common.JsonConverters;

public sealed class FullIncomeConverter : JsonConverter<FullIncome>
{
    public override FullIncome Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);

        var price = decimal.Parse(node["StaticIncome"]["AbsolutePrice"].ToString());
        var nominal = decimal.Parse(node["StaticIncome"]["AbsoluteNominal"].ToString());
        var couponIncome = node["CouponIncome"].Deserialize<CouponIncome>();
        var amortizationIncome = node["AmortizationIncome"].Deserialize<AmortizationIncome>();

        return new FullIncome(StaticIncome.FromAbsoluteValues(price, nominal), couponIncome, amortizationIncome);
    }

    public override void Write(Utf8JsonWriter writer, FullIncome value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
