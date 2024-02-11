using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Calculation.Dto.GetAmortization;

public class MoexAmortizationItem
{
    [JsonPropertyName("amortdate")]
    public DateOnly Date { get; set; }

    [JsonPropertyName("value_rub")]
    public decimal? Payment { get; set; }
}
