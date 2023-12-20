using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers.BondController.CalculateJson;

public sealed record CalculateJsonRequest(IEnumerable<Bond> Bonds);

public sealed record Bond(string Ticker,
                          string Name,
                          double Price,
                          DateTime EndDate,
                          double Denomination,
                          Coupon Coupon);

public sealed record Coupon(DateTime Date,
                            double Payout,
                            int PayRate);
