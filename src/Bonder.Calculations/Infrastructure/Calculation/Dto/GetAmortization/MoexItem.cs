using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetAmortization;

public class MoexItem
{
    [JsonPropertyName("amortizations")]
    public IEnumerable<MoexAmortizationItem> Amortizations { get; set; }

    [JsonPropertyName("coupons")]
    public IEnumerable<MoexCouponItem> Coupons { get; set; }
}