using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Domain.Common.Models;
using Domain.UserAggregate.ValueObjects;

namespace Domain.UserAggregate;

public class User : AggregateRoot<UserId>
{
    public MailAddress Email { get; }
    public string UserName { get; }
    public Tokens Tokens { get; }

    public User(UserId id, MailAddress email, Tokens? tokens = null) : base(id)
    {
        Email = email;
        Tokens = tokens ?? Tokens.Empty;
    }
}
