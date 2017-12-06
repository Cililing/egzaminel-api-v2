using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EgzaminelAPI.Models;
using EgzaminelAPI.Context;
using Microsoft.AspNetCore.Authorization;
using EgzaminelAPI.Helpers;

namespace EgzaminelAPI.Controllers
{
    public interface ISubjectClassController
    {
        SubjectGroup GetSubjectGroup(int id);
        ICollection<SubjectGroup> GetSubjectGroups(int parentId);
        ApiResponse AddSubjectGroup(int subjectId, SubjectGroup subjectGroup);
        ApiResponse UpdateSubject(int id, SubjectGroup subjectGroup);
        ApiResponse RemoveSubject(int id);
        IEnumerable<SubjectGroupEvent> GetSubjectGroupEvents(int id);
    }

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SubjectGroupsController : Controller, ISubjectClassController
    {
        private readonly ISubjectGroupsContext _subjectGroupContext;
        
        public SubjectGroupsController(ISubjectGroupsContext subjectGroupsContext)
        {
            this._subjectGroupContext = subjectGroupsContext;
        }

        [Route("{id}")]
        public SubjectGroup GetSubjectGroup(int id)
        {
            return _subjectGroupContext.GetSubjectGroup(id);
        }

        [Route("parent/{parentId}")]
        public ICollection<SubjectGroup> GetSubjectGroups(int parentId)
        {
            return _subjectGroupContext.GetSubjectGroups(parentId);
        }

        [Route("{id}/events")]
        public IEnumerable<SubjectGroupEvent> GetSubjectGroupEvents(int id)
        {
            return _subjectGroupContext.GetSubjectGroupEvents(new SubjectGroup { Id = id });
        }

        [Authorize(Policy = "TokenValidation")]
        [HttpPost]
        [Route("add/{subjectId}")]
        public ApiResponse AddSubjectGroup(int subjectId, [FromBody] SubjectGroup subjectGroup)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            subjectGroup.ParentSubject = new Subject { Id = subjectId };
            return _subjectGroupContext.AddSubjectGroup(subjectGroup, userToken);
        }

        [Authorize(Policy = "TokenValidation")]
        [HttpPost]
        [Route("update/{id}")]
        public ApiResponse UpdateSubject(int id, [FromBody] SubjectGroup subjectGroup)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            subjectGroup.Id = id;
            return _subjectGroupContext.EditSubjectGroup(subjectGroup, userToken);
        }

        [Authorize(Policy = "TokenValidation")]
        [Route("remove/{id}")]
        public ApiResponse RemoveSubject(int id)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            var subjectGroup = new SubjectGroup()
            {
                Id = id
            };

            return _subjectGroupContext.DeleteSubjectGroup(subjectGroup, userToken);
        }
    }
}