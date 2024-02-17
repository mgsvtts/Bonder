using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetAmortization;

public class MoexAmortizationItem
{
    [JsonPropertyName("amortdate")]
    public DateOnly Date { get; set; }

    [JsonPropertyName("value_rub")]
    public decimal? Payment { get; set; }
}