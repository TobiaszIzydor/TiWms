﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiWms.Application.ApplicationUser.DTOs;

namespace TiWms.Application.ApplicationUser.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<IEnumerable<ApplicationUserDto>>
    {
    }
}