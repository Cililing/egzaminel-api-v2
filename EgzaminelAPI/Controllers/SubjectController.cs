using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using EgzaminelAPI.Context;
using EgzaminelAPI.Models;

namespace EgzaminelAPI.Controllers
{
    public interface ISubjectController
    {
        Subject GetSubject (int id);
        ICollection<Subject> GetSubjects();
        ApiResponse AddSubject(string name, string desc, string password, int ownerId);
        ApiResponse UpdateSubject(int id, string name, string desc, string password);
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

    }
}