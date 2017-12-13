using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Helpers;
using EgzaminelAPI.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EgzaminelAPI.Context
{
    public interface IEgzaminelContext
    {
        MySqlConnection GetConnection();
    }

    public class EgzaminelContext : IEgzaminelContext
    {

        public string ConnectionString { get; set; }
        public EgzaminelContext(IConfig config)
        {
            ConnectionString = config.GetConnectionString();
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        protected User GetUser(string userToken, IRepo repo)
        {
            var userId = repo.GetToken(userToken)?.UserId;
            if (userId == null) FailOnAuth();

            return repo.GetUser(userId.Value);
        }

        protected void FailOnAuth()
        {
            throw new EgzaminelException();
        }

        protected bool CheckAnyPermissions(IEnumerable<Permission> permission, int objectId)
        {
            return CheckEditPermissions(permission, objectId) || CheckAdminPermissions(permission, objectId);
        }

        protected bool CheckEditPermissions(IEnumerable<Permission> permissions, int objectId)
        {
            var objectPermissions = permissions.Where(permission => permission.ObjectId == objectId);
            return (objectPermissions.Any() && (objectPermissions.First().HasAdminPermission || objectPermissions.First().CanModify));
        }

        protected bool CheckAdminPermissions(IEnumerable<Permission> permissions, int objectId)
        {
            var objectPermissions = permissions.Where(permission => permission.ObjectId == objectId);
            return (objectPermissions.Any() && objectPermissions.First().HasAdminPermission);
        }
    }
}
