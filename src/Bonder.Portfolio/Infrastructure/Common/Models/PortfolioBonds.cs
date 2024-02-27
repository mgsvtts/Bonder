using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("portfolio_bonds")]
public sealed class PortfolioBonds
{
    [Column("portfolio_id")]
    public Guid PortfolioId{ get; set; } 

    [Column("bond_id")]
    public Guid BondId{ get; set; }

    [Column("count")]
    public decimal Count{ get; set; }
}