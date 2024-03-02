namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct Dates
{
    public DateOnly? MaturityDate { get; init; }
    public DateOnly? OfferDate { get; init; }

    public Dates(DateOnly? maturityDate, DateOnly? offerDate = null)
    {
        MaturityDate = maturityDate;
        OfferDate = offerDate;
    }
}