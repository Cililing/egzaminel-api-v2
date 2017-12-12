using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Models;
using System.Collections.Generic;

namespace EgzaminelAPI.Context
{
    public interface IEventsContext : IEgzaminelContext
    {
        IEnumerable<GroupEvent> GetGroupEvents(int id);
        GroupEvent GetGroupEvent(int id);
        SubjectEvent GetSubjectEvent(int id);
        SubjectGroupEvent GetSubjectGroupEvent(int id);
        ApiResponse PostGroupEvent(GroupEvent groupEvent, string userToken);
        ApiResponse PostSubjectEvent(SubjectEvent subjectEvent, string userToken);
        ApiResponse PostSubjectGroupEvent(SubjectGroupEvent subjectGroupEvent, string userToken);
        ApiResponse UpdateGroupEvent(GroupEvent groupEvent, string userToken);
        ApiResponse UpdateSubjectEvent(SubjectEvent subjectEvent, string userToken);
        ApiResponse UpdateSubjectGroupEvent(SubjectGroupEvent subjectGroupEvent, string userToken);
        ApiResponse RemoveGroupEvent(GroupEvent groupEvent, string userToken);
        ApiResponse RemoveSubjectEvent(SubjectEvent subjectEvent, string userToken);
        ApiResponse RemoveSubjectGroupEvent(SubjectGroupEvent subjectGroupEvent, string userToken);
    }

    public class EventsContext : EgzaminelContext, IEventsContext
    {
        private readonly IRepo _repo;
        public EventsContext(IConfig config, IRepo repo) : base(config)
        {
            this._repo = repo;
        }

        public GroupEvent GetGroupEvent(int id)
        {
            return _repo.GetGroupEvent(id);
        }

        public SubjectEvent GetSubjectEvent(int id)
        {
            return _repo.GetSubjectEvent(id);
        }

        public SubjectGroupEvent GetSubjectGroupEvent(int id)
        {
            return _repo.GetSubjectGroupEvent(id);
        }

        public IEnumerable<GroupEvent> GetGroupEvents(int id)
        {
            return _repo.GetGroupEvents(id);
        }
        
        public ApiResponse PostGroupEvent(GroupEvent groupEvent, string userToken)
        {
            // User has to have group-edit permissions
            var user = GetUser(userToken, _repo);

            // Check edit permissions
            var hasPermission = CheckAnyPermissions(user.GroupsPermissions, groupEvent.ParentId);

            if (!hasPermission)
            {
                FailOnAuth();
            }

            return _repo.AddEvent(groupEvent);
        }

        public ApiResponse PostSubjectEvent(SubjectEvent subjectEvent, string userToken)
        {
            // User has to have group-edit permissions
            var user = GetUser(userToken, _repo);

            // Check edit permissions
            var hasPermission = CheckAnyPermissions(user.GroupsPermissions, _repo.GetSubjectParentId(subjectEvent.ParentId));
            if (!hasPermission)
            {
                FailOnAuth();
            }

            return _repo.AddEvent(subjectEvent);
        }

        public ApiResponse PostSubjectGroupEvent(SubjectGroupEvent subjectGroupEvent, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var hasPermission = CheckAnyPermissions(user.SubjectGroupsPermissions, subjectGroupEvent.ParentId);

            if (!hasPermission) FailOnAuth();

            return _repo.AddEvent(subjectGroupEvent);
        }

        public ApiResponse UpdateGroupEvent(GroupEvent groupEvent, string userToken)
        {
            var user = GetUser(userToken, _repo);

            // Check edit permissions
            var hasPermission = CheckAnyPermissions(user.GroupsPermissions, groupEvent.ParentId);

            if (!hasPermission)
            {
                FailOnAuth();
            }
            return _repo.EditEvent(groupEvent);
        }

        public ApiResponse UpdateSubjectEvent(SubjectEvent subjectEvent, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var hasPermission = CheckAnyPermissions(user.GroupsPermissions, _repo.GetSubjectParentId(subjectEvent.ParentId));

            if (!hasPermission) FailOnAuth();

            return _repo.EditEvent(subjectEvent);
        }

        public ApiResponse UpdateSubjectGroupEvent(SubjectGroupEvent subjectGroupEvent, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var hasPermission = CheckAnyPermissions(user.SubjectGroupsPermissions, subjectGroupEvent.ParentId);

            var groupId = _repo.GetSubjectGroupGroupId(subjectGroupEvent.Id);
            var hasGroupPermission = CheckAnyPermissions(user.GroupsPermissions, groupId);

            if (!hasPermission && !hasGroupPermission) FailOnAuth();

            return _repo.EditEvent(subjectGroupEvent);
        }

        public ApiResponse RemoveGroupEvent(GroupEvent groupEvent, string userToken)
        {
            var user = GetUser(userToken, _repo);
            groupEvent.ParentId = _repo.GetEventParentId(groupEvent);

            var hasPermission = CheckAnyPermissions(user.GroupsPermissions, groupEvent.ParentId);

            if (!hasPermission) FailOnAuth();

            return _repo.RemoveEvent(groupEvent);
        }

        public ApiResponse RemoveSubjectEvent(SubjectEvent subjectEvent, string userToken)
        {
            var user = GetUser(userToken, _repo);
            subjectEvent.ParentId = _repo.GetEventParentId(subjectEvent);

            var hasPermission = CheckAnyPermissions(user.GroupsPermissions, _repo.GetSubjectParentId(subjectEvent.ParentId));

            if (!hasPermission) FailOnAuth();

            return _repo.RemoveEvent(subjectEvent);
        }

        public ApiResponse RemoveSubjectGroupEvent(SubjectGroupEvent subjectGroupEvent, string userToken)
        {
            var user = GetUser(userToken, _repo);
            subjectGroupEvent.ParentId = _repo.GetEventParentId(subjectGroupEvent);

            var hasPermission = CheckAnyPermissions(user.SubjectGroupsPermissions, subjectGroupEvent.ParentId);

            var groupId = _repo.GetSubjectGroupGroupId(subjectGroupEvent.Id);
            var hasGroupPermission = CheckAnyPermissions(user.GroupsPermissions, groupId);

            if (!hasPermission && !hasGroupPermission) FailOnAuth();

            return _repo.RemoveEvent(subjectGroupEvent);
        }
    }
}
