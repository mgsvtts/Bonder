using Domain.BondAggreagte;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CalculateBonds;
public sealed record CalculateBondsCommand(IEnumerable<Bond> Bonds) : IRequest<CalculationResult>;
