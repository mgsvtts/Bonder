using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects.Users;
using Infrastructure.Common;
using Infrastructure.Common.Models;
using LinqToDB;
using LinqToDB.Data;
using Mapster;

namespace Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    public async Task SaveAsync(Domain.UserAggregate.User user, CancellationToken token)
    {
        var dbUser = user.Adapt<User>();
        var portfolioBonds = SetPortfolioValues(dbUser, user);

        using var db = new DbConnection();

        try
        {
            await db.BeginTransactionAsync(token);

            await db.Portfolios.Where(x => x.User.Token == dbUser.Token ||
                                            x.UserId == dbUser.Id)
            .DeleteAsync(token: token);

            await db.InsertOrReplaceAsync(dbUser, token: token);
            await db.BulkCopyAsync(dbUser.Portfolios, cancellationToken: token);
            await db.BulkCopyAsync(portfolioBonds, cancellationToken: token);
            await db.BulkCopyAsync(dbUser.Portfolios.SelectMany(x => x.Operations), cancellationToken: token);
            await db.BulkCopyAsync(dbUser.Portfolios.SelectMany(x => x.Operations).SelectMany(x => x.Trades), cancellationToken: token);

            await db.CommitTransactionAsync(token);
        }
        catch
        {
            await db.RollbackTransactionAsync(token);
            throw;
        }
    }

    public async Task DeleteAsync(UserId id, CancellationToken token)
    {
        using var db = new DbConnection();

        await db.Users
        .Where(x => x.Id == id.Value)
        .DeleteAsync(token: token);
    }

    public async Task<Domain.UserAggregate.User> GetOrCreateByIdAsync(UserId id, CancellationToken token)
    {
        using var db = new DbConnection();

        var user = await db.Users
        .LoadWith(x => x.Portfolios)
        .ThenLoad(x => x.Bonds)
        .LoadWith(x => x.Portfolios)
        .ThenLoad(x => x.Operations)
        .FirstOrDefaultAsync(x => x.Id == id.Value, token: token);

        if (user is null)
        {
            user = new User
            {
                Id = id.Value
            };

            await db.InsertAsync(user, token: token);
        }

        return user.Adapt<Domain.UserAggregate.User>();
    }

    public async Task<string> GetTokenAsync(UserId id, CancellationToken token)
    {
        using var db = new DbConnection();

        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id.Value, token)
        ?? throw new ArgumentException($"User {id.Value} does not have authorized token");

        return user.Token;
    }

    private static List<PortfolioBonds> SetPortfolioValues(User dbUser, Domain.UserAggregate.User user)
    {
        var result = new List<PortfolioBonds>();

        foreach (var portfolio in dbUser.Portfolios)
        {
            var domainPortfolio = user.Portfolios.First(x => x.Name.ToString() == portfolio.Name && x.Type == portfolio.Type);

            AddValues(dbUser.Id, domainPortfolio, result, portfolio);
        }

        return result;
    }

    private static void AddValues(Guid userId, Domain.UserAggregate.Entities.Portfolio domainPortfolio, List<PortfolioBonds> result, Portfolio portfolio)
    {
        portfolio.UserId = userId;

        foreach (var bond in domainPortfolio.Bonds)
        {
            result.Add(new PortfolioBonds
            {
                PortfolioId = portfolio.Id,
                BondId = bond.Id,
                Count = bond.Count
            });
        }

        foreach (var operation in portfolio.Operations)
        {
            operation.PortfolioId = portfolio.Id;

            foreach (var trade in operation.Trades)
            {
                trade.OperationId = operation.Id;
            }
        }
    }
}