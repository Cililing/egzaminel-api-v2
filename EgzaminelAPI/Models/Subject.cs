using EgzaminelAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EgzaminelAPI.Models
{
    [DataContract]
    public class Subject
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Description { get; set; }

        [DataMember(Order = 4)]
        public DateTime LastUpdate { get; set; }

        [DataMember(Order = 5)]
        public ICollection<SubjectEvent> Events { get; set; }

        [DataMember(Order = 6)]
        public ICollection<SubjectGroup> SubjectGroups { get; set; }

        [IgnoreDataMember]
        public Group ParentGroup { get; set; }
    }
}
