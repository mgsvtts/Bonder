using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dto.GetPortfolios;
public static class TinkoffOperationType
{
    public static string Input => "OPERATION_TYPE_INPUT";
    public static string Output => "OPERATION_TYPE_OUTPUT";
    public static string Tax => "OPERATION_TYPE_BOND_TAX";
    public static string CouponInput => "OPERATION_TYPE_COUPON";
}
