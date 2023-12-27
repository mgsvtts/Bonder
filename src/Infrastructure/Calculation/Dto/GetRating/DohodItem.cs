using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetRating;

public sealed class DohodItem
{
    [JsonPropertyName("credit_rating")]
    public string Rating { get; set; }

    [JsonPropertyName("isin")]
    public string Isin { get; set; }
}