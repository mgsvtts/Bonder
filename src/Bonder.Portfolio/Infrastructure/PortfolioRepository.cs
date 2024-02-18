using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects;
using Infrastructure.Common;
using LinqToDB;
using MapsterMapper;

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

    public async Task AttachToken(UserName userName, string token, CancellationToken cancellationToken = default)
    {
        await _db.InsertOrReplaceAsync(new Common.Models.User
        {
            UserName = userName.Name,
            Token = token
        }, token: cancellationToken);
    }
}