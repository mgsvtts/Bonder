using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;
using Domain.BondAggreagte.ValueObjects.Incomes;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.CalculateAll;
public sealed partial class CachedBondRepository
{
    public sealed class BondConverter : JsonConverter<Bond>
    {
        public override Bond Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonNode.Parse(ref reader);

            var price = decimal.Parse(node["Income"]["StaticIncome"]["AbsolutePrice"].ToString());
            var nominal = decimal.Parse(node["Income"]["StaticIncome"]["AbsoluteNominal"].ToString());
            var staticIncome = StaticIncome.FromAbsoluteValues(price, nominal);

            var couponIncome = JsonSerializer.Deserialize<CouponIncome>(node["Income"]["CouponIncome"]);
            var amortizationIncome = JsonSerializer.Deserialize<AmortizationIncome>(node["Income"]["AmortizationIncome"]);

            var constructor = typeToConvert
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .First();

            return (Bond)constructor.Invoke(
            [
                JsonSerializer.Deserialize<BondId>(node["Identity"]),
                node["Name"].ToString(),
                JsonSerializer.Deserialize<Dates>(node["Dates"]),
                JsonSerializer.Deserialize<int?>(node["Rating"]),
                JsonSerializer.Deserialize<IEnumerable<Coupon>>(node["Coupons"]),
                JsonSerializer.Deserialize<IEnumerable<Amortization>>(node["Amortizations"]),
                new FullIncome(staticIncome, couponIncome, amortizationIncome)
            ]);
        }

        public override void Write(Utf8JsonWriter writer, Bond value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
