using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Common.ValueObjects;
public readonly record struct ValidatedString
{
    private readonly string _value;

    public ValidatedString(string validatedString)
    {
        if (string.IsNullOrEmpty(validatedString))
        {
            throw new ArgumentNullException(nameof(validatedString), "String cannot be null or emtpy");
        }

        _value = validatedString;
    }

    public override string ToString()
    {
        return _value;
    }
}
