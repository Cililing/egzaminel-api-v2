using EgzaminelAPI.Context;
using EgzaminelAPI.Models;
using EgzaminelAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EgzaminelAPI.Helpers
{
    public interface IGroupsController
    {
        Group GetGroup(int id);
        ICollection<Group> GetGroups();
        ApiResponse AddGroup(string name, string desc, string password, int ownerId);
        ApiResponse UpdateGroup(int id, string name, string desc, string password);
        ApiResponse RemoveGroup(int id);
        IEnumerable<GroupEvent> GetGroupEvents(int id);
    }


    [Produces("application/json")]
    [Route("api/groups")]
    public class GroupsController : Controller, IGroupsController
    {
        private readonly IGroupsContext _groupsContext;
        private readonly IEventsContext _eventsContext;

        public GroupsController(IGroupsContext groupsContext, IEventsContext eventsContext)
        {
            _groupsContext = groupsContext;
            _eventsContext = eventsContext;
        }

        // GET api/groups/{id}
        [Route("{id}")]
        public  Group GetGroup(int id)
        {
            return _groupsContext.GetGroup(id);
        }

        // GET api/groups/user
        [Authorize(Policy = "TokenValidation")]
        [Route("user")]
        public ICollection<Group> GetGroups()
        {
            var token = this.GetAuthTokenFromHttpContext();
            return _groupsContext.GetGroups(token);
        }

        // GET api/groups/add
        [Authorize(Policy = "TokenValidation")]
        [Route("add")]
        public ApiResponse AddGroup(string name, string desc, string password, int ownerId)
        {
            var group = new Group()
            {
                Name = name,
                Description = desc,
                Password = password,
            };

            var token = this.GetAuthTokenFromHttpContext();

            return _groupsContext.AddGroup(group, token);
        }

        // GET api/groups/update/{id}
        [Authorize(Policy = "TokenValidation")]
        [Route("update/{id}")]
        public ApiResponse UpdateGroup(int id, string name, string desc, string password)
        {
            var group = new Group()
            {
                Id = id,
                Name = name,
                Description = desc,
                Password = password
            };

            var token = this.GetAuthTokenFromHttpContext();
            return _groupsContext.EditGroup(group, token);
        }

        // GET api/groups/remove/{id}
        [Authorize(Policy = "TokenValidation")]
        [Route("remove/{id}")]
        public ApiResponse RemoveGroup(int id)
        {
            var group = new Group()
            {
                Id = id,
            };

            var token = this.GetAuthTokenFromHttpContext();
            return _groupsContext.DeleteGroup(group, token);
        }

        // GET api/groups/{id}/events
        [Route("{id}/events")]
        public IEnumerable<GroupEvent> GetGroupEvents(int id)
        {
            return _eventsContext.GetGroupEvents(id);
        }

    }
}