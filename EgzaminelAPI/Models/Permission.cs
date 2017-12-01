using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EgzaminelAPI.Models
{
    [DataContract]
    public class Permission
    {
        public Permission() { }
        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public int ObjectId { get; set; }

        [DataMember(Order = 2)]
        public virtual Type PermissionType { get; private set; }

        [DataMember(Order = 3)]
        public bool HasAdminPermission { get; set; }

        [DataMember(Order = 4)]
        public bool CanModify { get; set; }

        [DataMember (Order = 5)]
        public DateTime LastUpdate { get; set; }
    }

    [DataContract]
    public class GroupPermission : Permission
    {
        public override Type PermissionType => GetType();
    }

    [DataContract]
    public class SubjectGroupPermission : Permission
    {
        public override Type PermissionType => GetType();
    }
}