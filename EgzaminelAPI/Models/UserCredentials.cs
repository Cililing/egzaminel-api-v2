using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EgzaminelAPI.Models
{
    public class UserCredentials
    {
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
    }
}
