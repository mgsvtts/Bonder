using System.Text.Json.Serialization;

namespace Infrastructure.Dto.GetPortfolios;

public sealed class TinkoffQuantity
{
    [JsonPropertyName("units")]
    public int Units { get; set; }

    [JsonPropertyName("nano")]
    public int Nanos { get; set; }

    public decimal ToDecimal()
    {
        return decimal.Parse(Units.ToString() + '.' + Nanos.ToString());
    }
}