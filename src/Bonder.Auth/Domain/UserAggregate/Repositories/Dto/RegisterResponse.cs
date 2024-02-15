using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.UserAggregate.Repositories.Dto;

public readonly record struct RegisterResponse(bool IsSuccess, IEnumerable<string> ErrorMessages);