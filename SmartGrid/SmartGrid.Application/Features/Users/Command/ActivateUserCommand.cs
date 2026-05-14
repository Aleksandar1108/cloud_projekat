using MediatR;
using SmartGrid.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Application.Features.Users.Command
{
    public class ActivateUserCommand(string token) : IRequest<Result<AuthResponse>>
    {
            //TODO

    }
}
