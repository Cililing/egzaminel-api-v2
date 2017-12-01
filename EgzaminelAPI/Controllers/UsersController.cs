using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EgzaminelAPI.Context;
using EgzaminelAPI.Auth;
using EgzaminelAPI.Models;

namespace EgzaminelAPI.Helpers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUsersContext _usersContext;
        private readonly ITokenService _tokenService;

        public UsersController(IUsersContext userContext, ITokenService tokenService)
        {
            this._usersContext = userContext;
            this._tokenService = tokenService;
        }

        [HttpGet]
        [Route("login")]
        public void LoginUser(string username, string password)
        {
            HttpContext.Response.Headers.Add("Authorization", _usersContext.ValidateUser(username, password));
        }

        //[Authorize(Policy = "CanModify")]
        [HttpGet]
        [Route("GetUser")]
        public User Test(int id)
        {
            return _usersContext.GetUser(id);
        }

    }
}