using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Domain;
using Domain.Exceptions;
using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.RegisterAsync
        (
            new User(new UserId(Guid.NewGuid()), new MailAddress(request.Email)),
            request.Password
        );

        if (!result.IsSuccess)
        {
            throw new AuthorizationException(string.Join(Environment.NewLine, result.ErrorMessages));
        }
    }
}