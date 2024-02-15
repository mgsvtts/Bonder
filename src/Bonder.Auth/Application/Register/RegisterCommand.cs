using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Register;

public record RegisterCommand(string UserName, string Password, string Email) : IRequest;