using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;
using Domain.BondAggreagte.ValueObjects.Incomes;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Infrastructure.Common.JsonConverters;

public sealed class BondConverter : JsonConverter<Bond>
{
    public override Bond Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);

        var constructor = typeToConvert
        .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
        .First();

        return (Bond)constructor.Invoke(
        [
            node["Identity"].Deserialize<BondId>(),
            node["Name"].ToString(),
            node["Dates"].Deserialize<Dates>(),
            node["Rating"].Deserialize<int?>(),
            node["Coupons"].Deserialize<IEnumerable<Coupon>>(),
            node["Amortizations"].Deserialize<IEnumerable<Amortization>>(),
            node["Income"].Deserialize<FullIncome>(options)
        ]);
    }

    public override void Write(Utf8JsonWriter writer, Bond value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
