using Application.Queries.GetStats;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers.Portfolios.Dto.GetStats;
public sealed record GetStatsRequest(Guid Id,
                                     StatType Type,
                                     DateTime DateFrom,
                                     DateTime DateTo);