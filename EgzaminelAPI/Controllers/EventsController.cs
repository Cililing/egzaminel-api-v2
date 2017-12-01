using Microsoft.AspNetCore.Mvc;
using EgzaminelAPI.Models;
using EgzaminelAPI.Context;
using EgzaminelAPI.Models;

namespace EgzaminelAPI.Helpers
{
    public interface IEventsController
    {
        GroupEvent GetGroupEvent(int id);
        SubjectEvent GetSubjectEvent(int id);
        SubjectGroupEvent GetSubjectGroupEvent(int id);

        ApiResponse PostGroupEvent(int parentId, string name, string description, string place, string date);
        ApiResponse PostSubjectEvent(int parentId, string name, string description, string place, string date);
        ApiResponse PostSubjectGroupEvent(int parentId, string name, string description, string place, string date);

        ApiResponse UpdateGroupEvent(int id, string name, string description, string place, string date);
        ApiResponse UpdateSubjectEvent(int id, string name, string description, string place, string date);
        ApiResponse UpdateSubjectGroupEvent(int id, string name, string description, string place, string date);

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
        [Route("group")]
        [HttpPost]
        public ApiResponse PostGroupEvent(
            int parentId,
            string name = "",
            string description = "",
            string place = "",
            string date = "")
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            return _eventContext.PostGroupEvent(
                new GroupEvent()
                {
                    Name = name,
                    Description = description,
                    Date = date.ConvertToDateTimeFromMySQLString(),
                    Place = place,
                    ParentId = parentId
                }, userToken);
        }

        // POST api/events/subject/
        [Route("subject")]
        [HttpPost]
        public ApiResponse PostSubjectEvent(
            int parentId,
            string name = "",
            string description = "",
            string place = "",
            string date = "")
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            return _eventContext.PostSubjectEvent(
                new SubjectEvent()
                {
                    Name = name,
                    Description = description,
                    Date = date.ConvertToDateTimeFromMySQLString(),
                    Place = place,
                    ParentId = parentId
                }, userToken);
        }

        // POST api/events/subjectGroup/
        [Route("subjectGroup")]
        [HttpPost]
        public ApiResponse PostSubjectGroupEvent(
            int parentId, 
            string name = "",
            string description = "",
            string place = "",
            string date = "")
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            return _eventContext.PostSubjectGroupEvent(
                new SubjectGroupEvent()
                {
                    Name = name,
                    Description = description,
                    Date = date.ConvertToDateTimeFromMySQLString(),
                    Place = place,
                    ParentId = parentId
                }, userToken);
        }

        [Route("group/{id}")]
        [HttpPut]
        public ApiResponse UpdateGroupEvent(
            int id,
            string name,
            string description,
            string place,
            string date)
        {
            var eventObj = _eventContext.GetGroupEvent(id);
            if (eventObj == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    ResultCode = -1
                };
            }
            var userToken = this.GetAuthTokenFromHttpContext();
            ControllerUtils.UpdateIfNotNull(id, () => eventObj.Name = name);
            ControllerUtils.UpdateIfNotNull(description, () => eventObj.Description = description);
            ControllerUtils.UpdateIfNotNull(place, () => eventObj.Place = place);
            ControllerUtils.UpdateIfNotNull(date, () => eventObj.Date = date.ConvertToDateTimeFromMySQLString());

            return _eventContext.UpdateGroupEvent(eventObj, userToken);
        }

        [Route("subject/{id}")]
        [HttpPut]
        public ApiResponse UpdateSubjectEvent(
            int id,
            string name,
            string description,
            string place,
            string date)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            var eventObj = _eventContext.GetSubjectEvent(id);
            if (eventObj == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    ResultCode = -1
                };
            }
            ControllerUtils.UpdateIfNotNull(id, () => eventObj.Name = name);
            ControllerUtils.UpdateIfNotNull(description, () => eventObj.Description = description);
            ControllerUtils.UpdateIfNotNull(place, () => eventObj.Place = place);
            ControllerUtils.UpdateIfNotNull(date, () => eventObj.Date = date.ConvertToDateTimeFromMySQLString());

            return _eventContext.UpdateSubjectEvent(eventObj, userToken);
        }

        [Route("subjectGroup/{id}")]
        [HttpPut]
        public ApiResponse UpdateSubjectGroupEvent(
            int id,
            string name,
            string description,
            string place,
            string date)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            var eventObj = _eventContext.GetSubjectGroupEvent(id);
            if (eventObj == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    ResultCode = -1
                };
            }
            ControllerUtils.UpdateIfNotNull(id, () => eventObj.Name = name);
            ControllerUtils.UpdateIfNotNull(description, () => eventObj.Description = description);
            ControllerUtils.UpdateIfNotNull(place, () => eventObj.Place = place);
            ControllerUtils.UpdateIfNotNull(date, () => eventObj.Date = date.ConvertToDateTimeFromMySQLString());

            return _eventContext.UpdateSubjectGroupEvent(eventObj, userToken);
        }

        [Route("group/{id}")]
        [HttpDelete]
        public ApiResponse DeleteGroupEvent(int id)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            return _eventContext.DeleteGroupEvent(new GroupEvent() { Id = id }, userToken);
        }

        [Route("subject/{id}")]
        [HttpDelete]
        public ApiResponse DeleteSubjectEvent(int id)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            return _eventContext.DeleteSubjectEvent(new SubjectEvent() { Id = id }, userToken);
        }

        [Route("subjectGroup/{id}")]
        [HttpDelete]
        public ApiResponse DeleteSubjectGroupEvent(int id)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            return _eventContext.DeleteSubjectGroupEvent(new SubjectGroupEvent() { Id = id }, userToken);
        }
    }
}