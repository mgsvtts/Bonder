using Domain.UserAggregate;
using Infrastructure.Common;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, User?>
{
    private readonly DatabaseContext _db;
    private readonly UserManager<Infrastructure.Common.Models.User> _userManager;

    public GetUserByIdQueryHandler(DatabaseContext db, UserManager<Infrastructure.Common.Models.User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async ValueTask<User?> Handle(GetUserByIdQuery request, CancellationToken token)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.Id.Value.ToString(), cancellationToken: token);

        if (user is null)
        {
            return null;
        }

        var claims = await _userManager.GetClaimsAsync(user);

        return (user, claims).Adapt<User>();
    }
}