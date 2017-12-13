using Microsoft.AspNetCore.Mvc;
using EgzaminelAPI.Models;
using EgzaminelAPI.Context;
using System;
using Microsoft.AspNetCore.Authorization;

namespace EgzaminelAPI.Helpers
{
    public interface IEventsController
    {
        GroupEvent GetGroupEvent(int id);
        SubjectEvent GetSubjectEvent(int id);
        SubjectGroupEvent GetSubjectGroupEvent(int id);

        ApiResponse PostGroupEvent(int parentId, GroupEvent eventObject);
        ApiResponse PostSubjectEvent(int parentId, SubjectEvent eventObject);
        ApiResponse PostSubjectGroupEvent(int parentId, SubjectGroupEvent eventObject);

        ApiResponse UpdateGroupEvent(int id, GroupEvent eventObject);
        ApiResponse UpdateSubjectEvent(int id, SubjectEvent eventObject);
        ApiResponse UpdateSubjectGroupEvent(int id, SubjectGroupEvent eventObject);

        ApiResponse DeleteGroupEvent(int id);
        ApiResponse DeleteSubjectEvent(int id);
        ApiResponse DeleteSubjectGroupEvent(int id);
    }
    
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EventsController : Controller, IEventsController
    {
        private readonly IEventsContext _eventContext;

        public EventsController(IEventsContext eventContext)
        {
            this._eventContext = eventContext;
        }

        [Route("group/{id}")]
        [HttpGet]
        public GroupEvent GetGroupEvent(int id)
        {
            return _eventContext.GetGroupEvent(id);
        }

        [Route("subject/{id}")]
        [HttpGet]
        public SubjectEvent GetSubjectEvent(int id)
        {
            return _eventContext.GetSubjectEvent(id);
        }

        [Route("subjectGroup/{id}")]
        [HttpGet]
        public SubjectGroupEvent GetSubjectGroupEvent(int id)
        {
            return _eventContext.GetSubjectGroupEvent(id);
        }

        // POST api/events/group/
        [Authorize(Policy = "TokenValidation")]
        [Route("group")]
        [HttpPost]
        public ApiResponse PostGroupEvent(int parentId, [FromBody] GroupEvent eventObject)
        {
            return PostEventObjectByType(parentId, eventObject);
        }

        // POST api/events/subject/
        [Authorize(Policy = "TokenValidation")]
        [Route("subject")]
        [HttpPost]
        public ApiResponse PostSubjectEvent(int parentId, [FromBody] SubjectEvent eventObject)
        {
            return PostEventObjectByType(parentId, eventObject);
        }

        // POST api/events/subjectGroup/
        [Authorize(Policy = "TokenValidation")]
        [Route("subjectGroup")]
        [HttpPost]
        public ApiResponse PostSubjectGroupEvent(int parentId, [FromBody] SubjectGroupEvent eventObject)
        {
            return PostEventObjectByType(parentId, eventObject);
        }

        [Authorize(Policy = "TokenValidation")]
        [Route("group/{id}")]
        [HttpPut]
        public ApiResponse UpdateGroupEvent(int id, [FromBody] GroupEvent eventObject)
        {
            return UpdateEventObjectByType(id, eventObject);
        }

        [Authorize(Policy = "TokenValidation")]
        [Route("subject/{id}")]
        [HttpPut]
        public ApiResponse UpdateSubjectEvent(int id, [FromBody] SubjectEvent eventObject)
        {
            return UpdateEventObjectByType(id, eventObject);
        }

        [Authorize(Policy = "TokenValidation")]
        [Route("subjectGroup/{id}")]
        [HttpPut]
        public ApiResponse UpdateSubjectGroupEvent(int id, [FromBody] SubjectGroupEvent eventObject)
        {
            return UpdateEventObjectByType(id, eventObject);
        }

        [Authorize(Policy = "TokenValidation")]
        [Route("group/{id}")]
        [HttpDelete]
        public ApiResponse DeleteGroupEvent(int id)
        {
            return DeleteEventObjectByType(id, new GroupEvent() { Id = id });
        }

        [Authorize(Policy = "TokenValidation")]
        [Route("subject/{id}")]
        [HttpDelete]
        public ApiResponse DeleteSubjectEvent(int id)
        {
            return DeleteEventObjectByType(id, new SubjectEvent() { Id = id });
        }

        [Authorize(Policy = "TokenValidation")]
        [Route("subjectGroup/{id}")]
        [HttpDelete]
        public ApiResponse DeleteSubjectGroupEvent(int id)
        {
            return DeleteEventObjectByType(id, new SubjectGroupEvent() { Id = id });
        }

        private ApiResponse PostEventObjectByType(int parentId, Event eventObject)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            eventObject.ParentId = parentId;
            return DAOUtils.DoByEventType<ApiResponse>(eventObject,
                () => _eventContext.PostGroupEvent(eventObject as GroupEvent, userToken),
                () => _eventContext.PostSubjectEvent(eventObject as SubjectEvent, userToken),
                () => _eventContext.PostSubjectGroupEvent(eventObject as SubjectGroupEvent, userToken));
        }

        private ApiResponse UpdateEventObjectByType(int id, Event eventObject)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            eventObject.Id = id;
            return DAOUtils.DoByEventType<ApiResponse>(eventObject,
                () => _eventContext.UpdateGroupEvent(eventObject as GroupEvent, userToken),
                () => _eventContext.UpdateSubjectEvent(eventObject as SubjectEvent, userToken),
                () => _eventContext.UpdateSubjectGroupEvent(eventObject as SubjectGroupEvent, userToken));
        }

        private ApiResponse DeleteEventObjectByType(int id, Event eventObject)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            eventObject.Id = id;
            return DAOUtils.DoByEventType<ApiResponse>(eventObject,
                () => _eventContext.RemoveGroupEvent(eventObject as GroupEvent, userToken),
                () => _eventContext.RemoveSubjectEvent(eventObject as SubjectEvent, userToken),
                () => _eventContext.RemoveSubjectGroupEvent(eventObject as SubjectGroupEvent, userToken));
        }
    }
}