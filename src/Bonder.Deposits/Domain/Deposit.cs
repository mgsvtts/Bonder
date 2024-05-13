using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;
public sealed class Deposit
{
    public float Rate { get;  }
    public Term Term { get; }

}

public readonly record struct Term(TermType Type, int Value);

public enum TermType
{
    Day,
    Month,
    Year
}
