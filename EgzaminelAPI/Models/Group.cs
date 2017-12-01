using EgzaminelAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace EgzaminelAPI.Models
{
    [DataContract]
    public class Group
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Description { get; set; }

        [DataMember(Order = 5)]
        public User Owner { get; set; }

        [DataMember(Order = 6)]
        public DateTime LastUpdate { get; set; }

        [DataMember(Order = 7)]
        public ICollection<GroupEvent> Events { get; set; }

        [DataMember(Order = 8)]
        public ICollection<Subject> Subjects { get; set; }

        [IgnoreDataMember]
        public string Password { get; set; }
    }
}
