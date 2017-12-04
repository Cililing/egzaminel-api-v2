using EgzaminelAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EgzaminelAPI.Models
{
    [DataContract]
    public class SubjectGroup 
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Place { get; set; }

        [DataMember(Order = 3)]
        public string Teacher { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember (Order = 5)]
        public DateTime LastUpdate { get; set; }

        [DataMember(Order = 6)]
        public ICollection<SubjectGroupEvent> Events { get; set; }

        [IgnoreDataMember]
        public Subject ParentSubject { get; set; }
    }
}
