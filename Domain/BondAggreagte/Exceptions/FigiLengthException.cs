namespace Domain.BondAggreagte.Exceptions;

public sealed class FigiLengthException(string value)
    : Exception($"Figi length must be equal to 12, you gave {value.Length}")
{
}