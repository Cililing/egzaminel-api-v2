using EgzaminelAPI.Auth;
using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Helpers;
using EgzaminelAPI.Models;
using System;
using System.Security.Cryptography;

namespace EgzaminelAPI.Context
{
    public interface IUsersContext
    {
        User GetUser(int id);
        ApiResponse RegisterUser(User userBasicData, string password);
        string ValidateUser(string username, string password);
    }


    public class UsersContext : EgzaminelContext, IUsersContext
    {
        private static readonly int ITERATIONS = 1000;

        private readonly ITokenService _tokenService;
        private readonly IRepo _repo;
        public UsersContext(IConfig config, IRepo repo, ITokenService tokenService) : base(config)
        {
            this._tokenService = tokenService;
            this._repo = repo;
        }

        public User GetUser(int id)
        {
            return _repo.GetUser(id);
        }

        public ApiResponse RegisterUser(User userBasicData, string password)
        {
            // Generate salt and hash
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, ITERATIONS);

            byte[] hash = pbkdf2.GetBytes(20);

            // convert them both
            string encryptedPassword = Convert.ToBase64String(hash);
            string saltString = Convert.ToBase64String(salt);

            userBasicData.Salt = saltString;
            userBasicData.EncryptedPassword = encryptedPassword;

            var answer = _repo.AddUser(userBasicData);
            if (answer.ResultCode > 0) answer.ResultCode = 0;

            return answer;
        }

        public string ValidateUser(string username, string password)
        {
            var hashedPassword = HashUserPassword(username, password);

            // Try to find user with such credentials
            var userId = _repo.GetUserId(username, hashedPassword);

            if (userId != null)
            {
                return _tokenService.GenerateToken(userId.Value).AuthToken;
            }

            return null;
        }

        private string HashUserPassword(string username, string password)
        {
            var userCredentials = _repo.GetUserCredentials(username);

            if (userCredentials == null) return null;

            // Hash entered password using salt from db
            byte[] userSalt = Convert.FromBase64String(userCredentials.Salt);
            var pdkdf2 = new Rfc2898DeriveBytes(password, userSalt, ITERATIONS);
            byte[] enteredHashedPassword = pdkdf2.GetBytes(20);
            var enteredHashedPasswordString = Convert.ToBase64String(enteredHashedPassword);

            return enteredHashedPasswordString;
        }
    }
}
