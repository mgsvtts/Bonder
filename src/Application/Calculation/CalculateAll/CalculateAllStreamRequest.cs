using Application.Calculation.Common.CalculationService.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Calculation.CalculateAll;
public sealed record CalculateAllStreamRequest() : IStreamRequest<CalculationResults>;