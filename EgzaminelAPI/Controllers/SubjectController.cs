using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using EgzaminelAPI.Context;
using EgzaminelAPI.Models;
using EgzaminelAPI.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace EgzaminelAPI.Controllers
{
    public interface ISubjectController
    {
        Subject GetSubject (int id);
        ICollection<Subject> GetSubjects(int parentId);
        ApiResponse AddSubject(int groupId, Subject subject);
        ApiResponse UpdateSubject(int id, Subject subject);
        ApiResponse RemoveSubject(int id);
        IEnumerable<SubjectEvent> GetSubjectEvents(int id);
    }


    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SubjectController : Controller, ISubjectController
    {
        private readonly ISubjectContext _subjectContext;

        public SubjectController(ISubjectContext subjectContext)
        {
            this._subjectContext = subjectContext;
        }

        [Route("{id}")]
        public Subject GetSubject(int id)
        {
            return _subjectContext.GetSubject(id);
        }

        [Route("parent/{id}")]
        public ICollection<Subject> GetSubjects(int id)
        {
            return _subjectContext.GetSubjects(id);
        }

        [Authorize(Policy = "TokenValidation")]
        [HttpPost]
        [Route("add/{groupId}")]
        public ApiResponse AddSubject(int groupId, [FromBody]Subject subject)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            subject.ParentGroup = new Group { Id = groupId };
            return _subjectContext.AddSubject(subject, userToken);
        }

        [Authorize(Policy = "TokenValidation")]
        [HttpPost]
        [Route("update/{id}")]
        public ApiResponse UpdateSubject(int id, [FromBody]Subject subject)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            subject.Id = id;
            return _subjectContext.EditSubject(subject, userToken);
        }

        [Authorize(Policy = "TokenValidation")]
        [Route("remove/{id}")]
        public ApiResponse RemoveSubject(int id)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            var subject = new Subject() { Id = id };
            return _subjectContext.DeleteSubject(subject, userToken);
        }
        
        [Route("{id}/events")]
        public IEnumerable<SubjectEvent> GetSubjectEvents(int id)
        {
            return _subjectContext.GetSubjectEvents(new Subject() { Id = id });
        }
    }
}