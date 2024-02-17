using Domain.Common.Models;
using Domain.UserAggregate.ValueObjects;
using System.Net.Mail;
using System.Security.Claims;

namespace Domain.UserAggregate;

public class User : AggregateRoot<UserId>
{
    private static readonly Claim _adminClaim = new("isAdmin", "true");

    private readonly List<Claim> _claims = [];

    public UserName UserName { get; }
    public MailAddress Email { get; }
    public Tokens Tokens { get; }
    public IReadOnlyList<Claim> Claims => _claims.AsReadOnly();
    public bool IsAdmin => _claims.Any(x => x.Type == _adminClaim.Type && x.Value == _adminClaim.Value);

    public User(UserId id,
                UserName userName,
                MailAddress email,
                IEnumerable<Claim>? claims = null,
                Tokens? tokens = null) : base(id)
    {
        _claims = claims is not null ? claims.ToList() : _claims;

        UserName = userName;
        Email = email;
        Tokens = tokens ?? Tokens.Empty;
    }
}