using Domain.BondAggreagte.Dto;
using Domain.Common.Exceptions;

namespace Domain.BondAggreagte.Exceptions;

public sealed class IntervalTypeException : DomainLogicException
{
    public IntervalTypeException() : base($"Interval with type {DateIntervalType.TillCustomDate} must have a date")
    {
    }
}