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
        ApiResponse AddSubjectGroup(string place, string teacher, string description, int subjectId);
        ApiResponse UpdateSubject(int id, string place, string teacher, string description);
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
        [Route("add")]
        public ApiResponse AddSubjectGroup(string place, string teacher, string description, int subjectId)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            var subjectGroup = new SubjectGroup()
            {
                Place = place,
                Teacher = teacher,
                Description = description,
                ParentSubject = new Subject { Id = subjectId }
            };

            return _subjectGroupContext.AddSubjectGroup(subjectGroup, userToken);
        }

        [Authorize(Policy = "TokenValidation")]
        [Route("update/{id}")]
        public ApiResponse UpdateSubject(int id, string place, string teacher, string description)
        {
            var userToken = this.GetAuthTokenFromHttpContext();
            var subjectGroup = new SubjectGroup()
            {
                Id = id,
                Place = place,
                Teacher = teacher,
                Description = description
            };

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