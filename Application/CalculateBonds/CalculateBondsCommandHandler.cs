using Domain.BondAggreagte;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CalculateBonds;
public sealed class CalculateBondsCommandHandler : IRequestHandler<CalculateBondsCommand, IOrderedEnumerable<CalculatedBond>>
{
    public async Task<IOrderedEnumerable<CalculatedBond>> Handle(CalculateBondsCommand request, CancellationToken cancellationToken)
    {
        var result = new List<CalculatedBond>();

        var couponIncomeList = request.Bonds.OrderByDescending(x => x.GetCouponOnlyIncome())
                                            .ToList();

        var priceList = request.Bonds.OrderBy(x => x.Money.Price)
                                     .ToList();

        var incomeList = request.Bonds.OrderByDescending(x => x.GetFullIncome())
                                      .ToList();

        foreach (var bond in request.Bonds)
        {
            var priceIndex = priceList.FindIndex(x => x.Id == bond.Id);
            var payoutIndex = couponIncomeList.FindIndex(x => x.Id == bond.Id);
            var mainIncome = incomeList.FindIndex(x => x.Id == bond.Id);
            result.Add(new CalculatedBond(bond, CalculatePriority(priceIndex, payoutIndex, mainIncome)));
        }

        return result.OrderBy(x => x.Priority);
    }

    private static int CalculatePriority(int priceIndex,
                                         int payoutIndex,
                                         int mainIncomeIndex)
    {
        return priceIndex + payoutIndex + mainIncomeIndex;
    }
}
