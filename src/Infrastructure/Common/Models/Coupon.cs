using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Models;
public sealed class Coupon
{
    [Key]
    public required Guid Id { get; set; }
    public required DateTime PaymentDate { get; set; }
    public required decimal Payout { get; set; }
    public required DateTime DividendCutOffDate { get; set; }
    public required bool IsFloating { get; set; }
}
