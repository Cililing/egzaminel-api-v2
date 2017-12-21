﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EgzaminelAPI.Context;
using EgzaminelAPI.Auth;
using EgzaminelAPI.Models;

namespace EgzaminelAPI.Helpers
{
    public interface IUserController
    {
        void LoginUser(string username, string password);
        ApiResponse RegisterUser(User user);
        ApiResponse EditUser(User user);
        ApiResponse DeleteUser(string username, string password);
    }

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UsersController : Controller, IUserController
    {
        private readonly IUsersContext _usersContext;
        private readonly ITokenService _tokenService;

        public UsersController(IUsersContext userContext, ITokenService tokenService)
        {
            this._usersContext = userContext;
            this._tokenService = tokenService;
        }

        public ApiResponse DeleteUser(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public ApiResponse EditUser(User user)
        {
            throw new System.NotImplementedException();
        }

        [HttpPost]
        [Route("register")]
        public ApiResponse RegisterUser([FromBody]User user)
        {
            // TODO send password as encrypted data
            var decryptedPassword = user.EncryptedPassword;

            return _usersContext.RegisterUser(user, decryptedPassword);
        }

        [HttpGet]
        [Route("login")]
        public void LoginUser(string username, string password)
        {
            // TODO send password as encrypted data
            HttpContext.Response.Headers.Add("Authorization", _usersContext.ValidateUser(username, password));
        }
    }
}