using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EgzaminelAPI.Models
{
    [DataContract]
    public class ResponseBase<T>
    {
        [DataMember(Order = 0)]
        public bool IsSuccess { get; set; }

    }
}
