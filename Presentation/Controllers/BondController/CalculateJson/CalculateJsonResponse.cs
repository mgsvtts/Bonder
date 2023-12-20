using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers.BondController.CalculateJson;
public sealed record CalculateJsonResponse(IEnumerable<CalculatedBondResponse> CalculatedBonds,
                                           IEnumerable<PriceBondResponse> PriceSortedBonds,
                                           IEnumerable<CouponeIncomeBondResponse> CouponIncomeSortedBonds,
                                           IEnumerable<IncomeBondResponse> IncomeSortedBonds);

public sealed record CalculatedBondResponse(string Ticker, string Name, int Priority);

public sealed record PriceBondResponse(string Ticker, string Name, double Price);

public sealed record CouponeIncomeBondResponse(string Ticker, string Name, double YearCouponIncome);

public sealed record IncomeBondResponse(string Ticker, string Name, double Income);