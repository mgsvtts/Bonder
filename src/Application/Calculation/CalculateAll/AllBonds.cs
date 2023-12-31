using Domain.BondAggreagte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Calculation.CalculateAll;
public static class AllBonds
{
    public static List<Bond> State { get; internal set; } = new List<Bond>();
}