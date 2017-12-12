using EgzaminelAPI.Auth;
using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Helpers;
using EgzaminelAPI.Models;

namespace EgzaminelAPI.Context
{
    public interface IUsersContext
    {
        User GetUser(int id);
        ApiResponse RegisterUser(string username, string password);
        ApiResponse EditUser(User user);
        ApiResponse RemoveUser(User user);

        string ValidateUser(string username, string password);
    }


    public class UsersContext : EgzaminelContext, IUsersContext
    {
        private readonly ITokenService _tokenService;
        private readonly IRepo _repo;
        public UsersContext(IConfig config, IRepo repo, ITokenService tokenService) : base(config)
        {
            this._tokenService = tokenService;
            this._repo = repo;
        }

        public ApiResponse EditUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public User GetUser(int id)
        {
            return _repo.GetUser(id);
        }

        public ApiResponse RegisterUser(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public ApiResponse RemoveUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public string ValidateUser(string username, string password)
        {
            // TODO GET ECRYPTED DATA
            var userId = _repo.GetUserId(username, password);

            if (userId == null)
            {
                throw new EgzaminelException();
            }

            return _tokenService.GenerateToken(userId.Value).AuthToken;
        }

    }
}
