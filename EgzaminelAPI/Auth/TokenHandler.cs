using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace EgzaminelAPI.Auth
{
    public interface ITokenHandler : IAuthorizationHandler
    {
    }

    public class TokenHandler : AuthorizationHandler<TokenRequirement>, ITokenHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;

        public TokenHandler(IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._tokenService = tokenService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
        {
            StringValues token;
            var success = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);

            if (!success)
            {
                return FailOnAuth(context);
            }

            bool isTokenValid = _tokenService.ValidateToken(token[0]);

            if (!isTokenValid)
            {
                return FailOnAuth(context);
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        private Task FailOnAuth(AuthorizationHandlerContext context)
        {
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
