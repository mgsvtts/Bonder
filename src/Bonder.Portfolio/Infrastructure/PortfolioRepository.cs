using Domain.UserAggregate;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects;
using Infrastructure.Common;
using Infrastructure.Common.Models;
using LinqToDB;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure;
public sealed class PortfolioRepository : IPortfolioRepository
{
    private readonly IMapper _mapper;
    private readonly DbConnection _db;

    public PortfolioRepository(DbConnection db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Domain.UserAggregate.User> AttachToken(UserName userName, string token, CancellationToken cancellationToken = default)
    {
        await _db.InsertOrReplaceAsync(new Common.Models.User
        {
            UserName = userName.Name,
            Token = token
        }, token: cancellationToken);

        var user = await _db.Users.FirstAsync(x => x.UserName == userName.Name, cancellationToken);

        return _mapper.Map<Domain.UserAggregate.User>(user);
    }
}
