using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EgzaminelAPI.Models
{
    [DataContract]
    public class ApiResponse
    {
        [DataMember(Order =1)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 2)]
        public int ResultCode { get; set; }
    }
}
