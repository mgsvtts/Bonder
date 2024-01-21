using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct OriginalMoney
{
    public decimal OriginalNominal { get; }
    public decimal OriginalPrice { get; }

    public OriginalMoney(decimal originalNominal, decimal originalPrice)
    {
        OriginalPrice = originalPrice;
        OriginalNominal = originalNominal;
    }
}
