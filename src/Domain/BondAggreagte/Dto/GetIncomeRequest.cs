using Domain.BondAggreagte.Exceptions;

namespace Domain.BondAggreagte.Dto;

public sealed class GetIncomeRequest
{
    public DateIntervalType Type { get; }
    public DateTime? TillDate { get; }
    public bool ConsiderDividendCutOffDate { get; }

    public GetIncomeRequest(DateIntervalType type,
                            DateTime? date = null,
                            bool considerDividendCutOffDate = true)
    {
        if (type == DateIntervalType.TillDate && date == null)
        {
            throw new IntervalTypeException();
        }

        Type = type;
        TillDate = date;
        ConsiderDividendCutOffDate = considerDividendCutOffDate;
    }

    public bool IsPaymentType()
    {
        return Type is DateIntervalType.TillMaturityDate or DateIntervalType.TillOfferDate;
    }
}
