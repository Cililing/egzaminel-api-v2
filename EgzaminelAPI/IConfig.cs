using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EgzaminelAPI
{
    public interface IConfig
    {
        string GetConnectionString();
        double GetTokenTime();
    }

    public class Config : IConfig
    {
        private readonly IConfiguration _configuration;

        public Config(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("EgzaminelDB");
        }

        public double GetTokenTime()
        {
            return 900;
        }
    }
}
