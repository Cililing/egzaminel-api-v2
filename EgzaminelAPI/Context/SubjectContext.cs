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
        //ICollection<Subject> GetSubjects(int parentId);
        ApiResponse AddSubject(Subject subject, string userToken);
        ApiResponse EditSubject(Subject subject, string userToken);
        ApiResponse DeleteSubject(Subject subject, string userToken);
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
            var hasPermission = this.CheckEditPermissions(user.GroupsPermissions, subject.ParentGroup.Id);

            if (!hasPermission) FailOnAuth();

            return _repo.PostSubject(subject);
        }

        public ApiResponse DeleteSubject(Subject subject, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var hasPermission = this.CheckEditPermissions(user.GroupsPermissions, subject.ParentGroup.Id);

            if (!hasPermission) FailOnAuth();

            return _repo.DeleteSubject(subject);
        }

        public ApiResponse EditSubject(Subject subject, string userToken)
        {
            var user = GetUser(userToken, _repo);
            var hasPermission = this.CheckEditPermissions(user.GroupsPermissions, subject.ParentGroup.Id);

            if (!hasPermission) FailOnAuth();

            return _repo.UpdateSubject(subject);
        }

        public Subject GetSubject(int id)
        {
            return _repo.GetSubject(id);
        }

        public ICollection<Subject> GetSubjects(int parentId)
        {
            return _repo.GetSubjects(parentId);
        }
    }
}
