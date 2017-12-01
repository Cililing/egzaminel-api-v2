using EgzaminelAPI.Auth;
using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Helpers;
using EgzaminelAPI.Models;

namespace EgzaminelAPI.Context
{
    public interface IUsersContext
    {
        User GetUser(int id);
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

        public User GetUser(int id)
        {
            return _repo.GetUser(id);
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
