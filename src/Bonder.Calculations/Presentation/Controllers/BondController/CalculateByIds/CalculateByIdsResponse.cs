using Presentation.Controllers.BondController.Calculate.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers.BondController.CalculateByIds;
public readonly record struct CalculateByIdsResponse(CalculateResponse CalculateResponse, IEnumerable<string> NotFound);