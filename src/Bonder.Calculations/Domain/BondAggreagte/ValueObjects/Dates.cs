namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct Dates
{
    public DateOnly? MaturityDate { get; }
    public DateOnly? OfferDate { get; }

    public Dates(DateOnly? maturityDate, DateOnly? offerDate = null)
    {
        MaturityDate = maturityDate;
        OfferDate = offerDate;
    }
}