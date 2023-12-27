using Domain.BondAggreagte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Calculation.Common.CalculationService.Dto;
public readonly record struct CalculationMoneyResult(Bond Bond, decimal Money);
