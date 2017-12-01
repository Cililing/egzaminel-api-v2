using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EgzaminelAPI.Auth
{
    public class TokenRequirement : IAuthorizationRequirement
    {
        public TokenRequirement()
        {
        }
    }
}
