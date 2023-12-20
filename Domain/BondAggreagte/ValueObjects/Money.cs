using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte.ValueObjects;
public class Money
{
    public double DenominationIncome { get; private set; }

    public double Price { get; private set; }

    public Money(double price, double denomination)
    {
        DenominationIncome = denomination - price;
        Price = price;
    }
}