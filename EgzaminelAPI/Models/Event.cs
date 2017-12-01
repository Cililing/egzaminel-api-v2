using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EgzaminelAPI.Models
{
    [DataContract]
    public class Event
    {
        [DataMember(Order = 1)]
        public int? Id { get; set; }

        [DataMember(Order = 2)]
        public virtual Type EventType { get; private set; }

        [DataMember(Order = 3)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember(Order = 5)]
        public DateTime? Date { get; set; }

        [DataMember(Order = 6)]
        public string Place { get; set; }

        [DataMember(Order = 7)]
        public DateTime LastUpdate { get; set; }

        [DataMember(Order = 8)]
        public int ParentId { get; set; }
    }

    [DataContract]
    public class GroupEvent : Event
    {
        public override Type EventType => GetType();
    }

    [DataContract]
    public class SubjectEvent: Event
    {
        public override Type EventType => GetType();
    }

    [DataContract]
    public class SubjectGroupEvent : Event
    {
        public override Type EventType => GetType();
    }
    

}
