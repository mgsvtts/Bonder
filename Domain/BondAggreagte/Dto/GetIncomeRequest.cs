using Domain.BondAggreagte.Exceptions;

namespace Domain.BondAggreagte.Dto;

public sealed class GetIncomeRequest
{
    public DateIntervalType Type { get; }
    public DateTime? Date { get; }

    public GetIncomeRequest(DateIntervalType type, DateTime? date = null)
    {
        if (type == DateIntervalType.TillDate && date == null)
        {
            throw new IntervalTypeException();
        }

        Type = type;
        Date = date;
    }
}