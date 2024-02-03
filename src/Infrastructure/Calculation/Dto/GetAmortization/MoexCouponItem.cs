using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetAmortization;

public class MoexCouponItem
{
    [JsonPropertyName("coupondate")]
    public DateOnly Date { get; set; }

    [JsonPropertyName("recorddate")]
    public DateOnly CutOffDate { get; set; }

    [JsonPropertyName("value")]
    public decimal? Payout { get; set; }
}