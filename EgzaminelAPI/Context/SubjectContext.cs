using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EgzaminelAPI.Context
{
    public interface ISubjectContext : IEgzaminelContext
    {
        Subject GetSubject(int id);
        ICollection<Subject> GetSubjects(int parentId);
        ApiResponse AddSubject(Subject subject, string userToken);
        ApiResponse EditSubject(Subject subject, string userToken);
        ApiResponse DeleteSubject(Subject subject, string userToken);
        ICollection<SubjectEvent> GetSubjectEvents(Subject subject);
    }

    public class SubjectContext : EgzaminelContext, ISubjectContext
    {

        private readonly IRepo _repo;

        public SubjectContext(IConfig config, IRepo repo) : base(config)
        {
            this._repo = repo;
        }

        public ApiResponse AddSubject(Subject subject, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var hasPermission = this.CheckEditPermissions(user.GroupsPermissions, subject.ParentGroup.Id.Value);

            if (!hasPermission) FailOnAuth();

            return _repo.AddSubject(subject);
        }

        public ApiResponse DeleteSubject(Subject subject, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var groupId = _repo.GetSubjectParentId(subject.Id);
            var hasPermission = this.CheckEditPermissions(user.GroupsPermissions, groupId);

            if (!hasPermission) FailOnAuth();

            return _repo.DeleteSubject(subject);
        }

        public ApiResponse EditSubject(Subject subject, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var groupId = _repo.GetSubjectParentId(subject.Id);
            var hasPermission = this.CheckEditPermissions(user.GroupsPermissions, groupId);

            if (!hasPermission) FailOnAuth();

            return _repo.EditSubject(subject);
        }

        public Subject GetSubject(int id)
        {
            return _repo.GetSubject(id);
        }

        public ICollection<Subject> GetSubjects(int parentId)
        {
            return _repo.GetSubjects(parentId);
        }

        public ICollection<SubjectEvent> GetSubjectEvents(Subject subject)
        {
            return _repo.GetSubjectEvents(subject.Id);
        }
    }
}
