using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Helpers;
using System;

namespace EgzaminelAPI.Auth
{
    public interface ITokenService
    {
        TokenModel GenerateToken(int userId);
        bool ValidateToken(string tokenId);
        bool KillToken(string tokenId);
        bool DeleteTokenByUserId(int tokenId);
    }


    public class TokenService : ITokenService
    {
        private readonly IConfig _config;
        private readonly IRepo _repository;

        public TokenService(IConfig config, IRepo repo)
        {
            this._config = config;
            this._repository = repo;
        }

        public TokenModel GenerateToken(int userId)
        {
            string token = Guid.NewGuid().ToString();

            var tokenObject = new TokenModel()
            {
                UserId = userId,
                AuthToken = token,
            };

            RefreshTokenTime(ref tokenObject);

            if (!_repository.SaveToken(tokenObject))
            {
                throw new EgzaminelException();
            }

            return tokenObject;
        }

        public bool ValidateToken(string token)
        {
            var tokenEntity = _repository.GetToken(token);

            if (tokenEntity?.ExpiresOn > DateTime.Now)
            {
                RefreshTokenTime(ref tokenEntity);
                _repository.UpdateToken(tokenEntity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool KillToken(string tokenId)
        {
            return _repository.DeleteToken(tokenId);
        }

        public bool DeleteTokenByUserId(int userId)
        {
            return _repository.DeleteTokenByUserId(userId);
        }

        private void RefreshTokenTime(ref TokenModel tokenModel)
        {
            DateTime issuedOn = DateTime.Now;
            DateTime expiresOn = DateTime.Now.AddSeconds(_config.GetTokenTime());

            tokenModel.IssuedOn = issuedOn;
            tokenModel.ExpiresOn = expiresOn;
        }
    }
}
