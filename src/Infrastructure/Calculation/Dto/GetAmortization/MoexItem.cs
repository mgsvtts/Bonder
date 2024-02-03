using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Calculation.Dto.GetAmortization;

public class MoexItem
{
    [JsonPropertyName("amortizations")]
    public IEnumerable<MoexAmortizationItem> Amortizations { get; set; }

    [JsonPropertyName("coupons")]
    public IEnumerable<MoexCouponItem> Coupons { get; set; }
}