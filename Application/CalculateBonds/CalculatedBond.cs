using Domain.BondAggreagte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CalculateBonds;
public sealed record CalculatedBond(Bond Bond, int Priority);