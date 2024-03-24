using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers.Dto.GetStats;
public readonly record struct GetStatsResponse(decimal Amount, decimal Percents);