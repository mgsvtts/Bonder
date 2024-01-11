using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;
using System.Collections.Concurrent;

namespace Application.Calculation.CalculateAll;

public static class AllBonds
{
    private static readonly ConcurrentDictionary<BondId, Bond> _state = new ConcurrentDictionary<BondId, Bond>();

    public static IReadOnlyDictionary<BondId, Bond> State => _state.AsReadOnly();

    public static void AddOrUpdate(IEnumerable<Bond> bonds)
    {
        foreach (var bond in bonds)
        {
            _state.AddOrUpdate(bond.Identity, bond, (bondId, oldBond) => bond);
        }
    }
}