using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EgzaminelAPI.Models
{
    [DataContract]
    public class User
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 1)]
        public string Username { get; set; }

        [DataMember(Order = 3)]
        public DateTime LastUpdate { get; set; }

        [DataMember(Order = 4)]
        public IEnumerable<GroupPermission> GroupsPermissions { get; set; }

        [DataMember(Order = 5)]
        public IEnumerable<SubjectGroupPermission> SubjectGroupsPermissions { get; set; }

        [DataMember(Order = 6)]
        public string EncryptedPassword { get; set; }

        [DataMember(Order = 7)]
        public string Email { get; set; }

        [IgnoreDataMember]
        public string Salt { get; set; }
    }
}
