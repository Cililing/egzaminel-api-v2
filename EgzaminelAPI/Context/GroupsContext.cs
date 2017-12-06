using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace EgzaminelAPI.Context
{
    public interface IGroupsContext : IEgzaminelContext
    {
        Group GetGroup(int id);
        ICollection<Group> GetGroups(string userToken);
        ApiResponse AddGroup(Group group, string userToken);
        ApiResponse EditGroup(Group group, string userToken);
        ApiResponse DeleteGroup(Group group, string userToken);
    }

    public class GroupsContext : EgzaminelContext, IGroupsContext
    {
        private readonly IRepo _repo;
        public GroupsContext(IConfig config, IRepo repo) : base(config)
        {
            this._repo = repo;
        }
        public Group GetGroup(int id)
        {
            return _repo.GetGroup(id);
        }

        public ICollection<Group> GetGroups(string userToken)
        {
            var user = GetUser(userToken, _repo);
            if (user == null) FailOnAuth();

            return _repo.GetGroups(user);
        }

        public ApiResponse AddGroup(Group group, string userToken)
        {
            var user = GetUser(userToken, _repo);
            if (user == null) FailOnAuth();

            group.Owner = user;
            var result = _repo.AddGroup(group, user.Id);
            _repo.CreartePermission(new GroupPermission()
            {
                UserId = user.Id,
                ObjectId = result.ResultCode,
                CanModify = true,
                HasAdminPermission = true
            });

            return result;
        }

        public ApiResponse EditGroup(Group group, string userToken)
        {
            var user = GetUser(userToken, _repo);

            // check data
            if (user == null || user.GroupsPermissions == null) FailOnAuth();

            // check permission
            var permissions = user.GroupsPermissions.Where(x => x.ObjectId == group.Id);

            if (CheckEditPermissions(user.GroupsPermissions, group.Id.Value))
            {
                return _repo.EditGroup(group);
            }
            else
            {
                FailOnAuth();
                return null;
            }

        }

        public ApiResponse DeleteGroup(Group group, string userToken)
        {
            var user = GetUser(userToken, _repo);

            if (user == null || user.GroupsPermissions == null) FailOnAuth();

            if (CheckAdminPermissions(user.GroupsPermissions, group.Id.Value))
            {
                return _repo.DeleteGroup(group);
            }
            else
            {
                FailOnAuth();
                return null;
            }
        }
    }
}
