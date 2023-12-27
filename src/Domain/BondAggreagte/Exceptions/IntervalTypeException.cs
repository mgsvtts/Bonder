using Domain.BondAggreagte.Dto;

namespace Domain.BondAggreagte.Exceptions;

public sealed class IntervalTypeException : Exception
{
    public IntervalTypeException() : base($"Interval with type {DateIntervalType.TillDate} must have a date")
    {
    }
}