using Domain.BondAggreagte;

namespace Application.Calculation.CalculateAll;

public static class AllBonds
{
    private static readonly List<Bond> _state = new List<Bond>();

    public static IReadOnlyList<Bond> State => _state.AsReadOnly();

    public static void AddOrUpdate(IEnumerable<Bond> bonds)
    {
        _state.RemoveAll(x => bonds.Select(x => x.Id).Contains(x.Id));

        _state.AddRange(bonds);

        _state.RemoveAll(x => x.Money.Price <= 0);
    }
}