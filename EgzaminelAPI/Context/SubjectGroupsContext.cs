using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EgzaminelAPI.Context
{
    public interface ISubjectGroupsContext
    {
        SubjectGroup GetSubjectGroup(int id);
        ICollection<SubjectGroup> GetSubjectGroups(int parentId);
        ApiResponse AddSubjectGroup(SubjectGroup subjectGroup, string userToken);
        ApiResponse EditSubjectGroup(SubjectGroup subjectGroup, string userToken);
        ApiResponse DeleteSubjectGroup(SubjectGroup subjectGroup, string userToken);
        ICollection<SubjectGroupEvent> GetSubjectGroupEvents(SubjectGroup subjectGroup);
    }

    public class SubjectGroupsContext : EgzaminelContext, ISubjectGroupsContext
    {
        private IRepo _repo;
        public SubjectGroupsContext(IConfig config, IRepo repo) : base(config)
        {
            this._repo = repo;
        }

        public SubjectGroup GetSubjectGroup(int id)
        {
            return _repo.GetSubjectGroup(id);
        }

        public ICollection<SubjectGroup> GetSubjectGroups(int parentId)
        {
            return _repo.GetSubjectGroups(parentId);
        }

        public ApiResponse AddSubjectGroup(SubjectGroup subjectGroup, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var hasSubjectGroupPermission = this.CheckEditPermissions(user.SubjectGroupsPermissions, subjectGroup.Id);

            var groupId = _repo.GetSubjectParentId(subjectGroup.ParentSubject.Id);
            var hasGroupPermission = this.CheckAnyPermissions(user.GroupsPermissions, groupId);
            
            if (!hasSubjectGroupPermission && !hasGroupPermission)
            {
                FailOnAuth();
            }

            return _repo.AddSubjectGroup(subjectGroup);
        }

        public ApiResponse DeleteSubjectGroup(SubjectGroup subjectGroup, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var hasSubjectGroupPermission = this.CheckEditPermissions(user.SubjectGroupsPermissions, subjectGroup.Id);

            var groupId = _repo.GetSubjectGroupGroupId(subjectGroup.Id);
            var hasGroupPermission = this.CheckAnyPermissions(user.GroupsPermissions, groupId);

            if (!hasSubjectGroupPermission && !hasGroupPermission)
            {
                FailOnAuth();
            }

            return _repo.RemoveSubjectGroup(subjectGroup);
        }

        public ApiResponse EditSubjectGroup(SubjectGroup subjectGroup, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var hasSubjectGroupPermission = this.CheckEditPermissions(user.SubjectGroupsPermissions, subjectGroup.Id);

            var groupId = _repo.GetSubjectGroupGroupId(subjectGroup.Id);
            var hasGroupPermission = this.CheckAnyPermissions(user.GroupsPermissions, groupId);

            if (!hasSubjectGroupPermission && !hasGroupPermission)
            {
                FailOnAuth();
            }

            return _repo.EditSubjectGroup(subjectGroup);
        }

        public ICollection<SubjectGroupEvent> GetSubjectGroupEvents(SubjectGroup subjectGroup)
        {
            return _repo.GetSubjectGroupEvents(subjectGroup.Id);
        }

    }
}
