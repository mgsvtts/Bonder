using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects;
using Infrastructure.Common;
using Infrastructure.Common.Models;
using LinqToDB;
using LinqToDB.Data;
using Mapster;

namespace Infrastructure;

public sealed class UserRepository : IUserRepository
{
    private readonly DbConnection _db;

    public UserRepository(DbConnection db)
    {
        _db = db;
    }

    public async Task SaveAsync(Domain.UserAggregate.User user, CancellationToken token = default)
    {
        var dbUser = user.Adapt<User>();
        var portfolioBonds = SetPortfolioValues(dbUser, user);

        try
        {
            await _db.BeginTransactionAsync(token);

            await _db.Portfolios.Where(x => x.Id == dbUser.Id)
            .DeleteAsync(token: token);

            await _db.InsertOrReplaceAsync(dbUser, token: token);
            await _db.BulkCopyAsync(dbUser.Portfolios, cancellationToken: token);
            await _db.BulkCopyAsync(portfolioBonds, cancellationToken: token);

            await _db.CommitTransactionAsync(token);
        }
        catch
        {
            await _db.RollbackTransactionAsync(token);

            throw;
        }
    }

    public async Task DeleteAsync(UserId id, CancellationToken token = default)
    {
        await _db.Users
        .Where(x => x.Id == id.Value)
        .DeleteAsync(token: token);
    }

    public async Task<Domain.UserAggregate.User> GetByIdAsync(UserId id, CancellationToken token = default)
    {
        var user = await _db.Users
        .LoadWith(x => x.Portfolios)
        .ThenLoad(x => x.Bonds)
        .FirstOrDefaultAsync(x => x.Id == id.Value, token: token)
        ?? throw new ArgumentException($"User {id.Value} does not have authorized token");

        return user.Adapt<Domain.UserAggregate.User>();
    }

    public async Task<string> GetTokenAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken)
        ?? throw new ArgumentException($"User {id.Value} does not have authorized token");

        return user.Token;
    }

    private static List<PortfolioBonds> SetPortfolioValues(User dbUser, Domain.UserAggregate.User user)
    {
        var result = new List<PortfolioBonds>();

        foreach (var portfolio in dbUser.Portfolios)
        {
            portfolio.UserId = dbUser.Id;
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