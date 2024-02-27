
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects;
using Infrastructure.Common;
using Infrastructure.Common.Models;
using LinqToDB;
using LinqToDB.Data;
using Mapster;
using MapsterMapper;

namespace Infrastructure;

public sealed class UserRepository : IUserRepository
{
    private readonly DbConnection _db;

    public UserRepository(DbConnection db)
    {
        _db = db;
    }

    public async Task AddAsync(Domain.UserAggregate.User user, CancellationToken cancellationToken = default)
    {
        var dbUser = user.Adapt<Common.Models.User>();
        var portfolioBonds = SetPortfolioValues(dbUser, user);

        try
        {
            await _db.BeginTransactionAsync(cancellationToken);

            await _db.Portfolios.Where(x => x.UserName == dbUser.UserName)
            .DeleteAsync(token: cancellationToken);

            await _db.InsertOrReplaceAsync(dbUser, token: cancellationToken);
            await _db.BulkCopyAsync(dbUser.Portfolios, cancellationToken: cancellationToken);
            await _db.BulkCopyAsync(portfolioBonds, cancellationToken: cancellationToken);

            await _db.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _db.RollbackTransactionAsync(cancellationToken);

            throw;
        }
    }

    public async Task<Domain.UserAggregate.User> GetByUserNameAsync(UserName userName, CancellationToken token = default)
    {
        var user = await _db.Users
        .LoadWith(x=>x.Portfolios)
        .ThenLoad(x=>x.Bonds)
        .FirstOrDefaultAsync(x => x.UserName == userName.Name, token: token)
        ?? throw new ArgumentException($"User {userName.Name} does not have authorized token");

        return user.Adapt<Domain.UserAggregate.User>();
    }

    public async Task<string> GetTokenAsync(UserName userName, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == userName.Name, cancellationToken)
        ?? throw new ArgumentException($"User {userName.Name} does not have authorized token");

        return user.Token;
    }

    private static List<PortfolioBonds> SetPortfolioValues(Common.Models.User dbUser, Domain.UserAggregate.User user)
    {
        var result = new List<PortfolioBonds>();

        foreach (var portfolio in dbUser.Portfolios)
        {
            portfolio.UserName = dbUser.UserName;
            var domainPortfolio = user.Portfolios.First(x => x.Name == portfolio.Name && x.Type == portfolio.Type);
            foreach (var bond in domainPortfolio.Bonds)
            {
                result.Add(new PortfolioBonds
                {
                    PortfolioId = portfolio.Id,
                    BondId = bond.Id,
                    Count = bond.Count
                });
            }
        }

        return result;
    }
}