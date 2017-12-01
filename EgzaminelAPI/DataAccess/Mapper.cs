using EgzaminelAPI.Auth;
using EgzaminelAPI.Models;
using EgzaminelAPI.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace EgzaminelAPI.DataAccess
{
    public interface IMapper
    {
        TokenModel MapToken(MySqlDataReader reader);
        int? MapUserId(MySqlDataReader reader);
        User MapUser(MySqlDataReader reader);
        ICollection<User> MapUsers(MySqlDataReader reader);
        int? MapSubjectParentId(MySqlDataReader reader);
        IEnumerable<T> MapPermissions<T>(MySqlDataReader reader) where T : Permission, new();


        // DATA
        ICollection<Group> MapGroups(MySqlDataReader reader);
        ICollection<Subject> MapSubjects(MySqlDataReader reader);
        ICollection<SubjectGroup> MapSubjectGroups(MySqlDataReader reader);
        ICollection<GroupEvent> MapGroupEvents(MySqlDataReader reader);
        ICollection<SubjectEvent> MapSubjectEvents(MySqlDataReader reader);
        ICollection<SubjectGroupEvent> MapSubjectGroupEvents(MySqlDataReader reader);

        Group MapGroup(MySqlDataReader reader);
        Subject MapSubject(MySqlDataReader reader);
        SubjectGroup MapSubjectGroup(MySqlDataReader reader);
        GroupEvent MapGroupEvent(MySqlDataReader reader);
        SubjectEvent MapSubjectEvent(MySqlDataReader reader);
        SubjectGroupEvent MapSubjectGroupEvent(MySqlDataReader reader);
    }

    public class Mapper : IMapper
    {
        public Mapper()
        {

        }
        // DATA

        public ICollection<User> MapUsers(MySqlDataReader reader)
        {
            return MapMySqlList(reader, () => MapUser(reader));
        }

        public IEnumerable<T> MapPermissions<T>(MySqlDataReader reader) where T : Permission, new()
        {
            return MapMySqlList(reader, () => MapPermission<T>(reader));
        }
        public ICollection<Group> MapGroups(MySqlDataReader reader)
        {
            return MapMySqlList(reader, () => MapGroup(reader));
        }

        public ICollection<Subject> MapSubjects(MySqlDataReader reader)
        {
            return MapMySqlList(reader, () => MapSubject(reader));
        }

        public ICollection<SubjectGroup> MapSubjectGroups(MySqlDataReader reader)
        {
            return MapMySqlList(reader, () => MapSubjectGroup(reader));
        }

        public ICollection<GroupEvent> MapGroupEvents(MySqlDataReader reader)
        {
            return MapMySqlList(reader, () => MapGroupEvent(reader));
        }

        public ICollection<SubjectEvent> MapSubjectEvents(MySqlDataReader reader)
        {
            return MapMySqlList(reader, () => MapSubjectEvent(reader));
        }

        public ICollection<SubjectGroupEvent> MapSubjectGroupEvents(MySqlDataReader reader)
        {
            return MapMySqlList(reader, () => MapSubjectGroupEvent(reader));
        }

        public int? MapUserId(MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                return GetSafeValue(reader, "id", () => reader.GetInt32("id"));
            }
            else
            {
                return null;
            }
        }

        public int? MapSubjectParentId(MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                return GetSafeValue(reader, "group_id", () => reader.GetInt32("group_id"));
            }
            else
            {
                return null;
            }
        }

        public TokenModel MapToken(MySqlDataReader reader)
        {
            return new TokenModel()
            {
                AuthToken = GetSafeValue(reader, "auth_token", () => reader["auth_token"].ToString()),
                UserId = GetSafeValue(reader, "user_id", () => reader.GetInt32("user_id")),
                IssuedOn = GetSafeValue(reader, "issued_on", () => Convert.ToDateTime(reader["issued_on"])),
                ExpiresOn = GetSafeValue(reader, "expires_on", () => Convert.ToDateTime(reader["expires_on"]))
            };
        }

        public T MapPermission<T>(MySqlDataReader reader) where T : Permission, new()
        {
            return new T()
            {
                UserId = GetSafeValue(reader, "user_id", () => reader.GetInt32("user_id")),
                ObjectId = GetSafeValue(reader, "object_id", () => reader.GetInt32("object_id")),
                HasAdminPermission = GetSafeValue(reader, "has_admin_permission", () => reader.GetBoolean("has_admin_permission")),
                CanModify = GetSafeValue(reader, "can_modify", () => reader.GetBoolean("can_modify")),
                LastUpdate = GetSafeValue(reader, "last_update", () => Convert.ToDateTime(reader["last_update"]))
            };
        }

        public User MapUser(MySqlDataReader reader)
        {
            return new User()
            {
                Id = GetSafeValue(reader, "id", () => reader.GetInt32("id")),
                Username = GetSafeValue(reader, "username", () => reader["username"].ToString()),
                Email = GetSafeValue(reader, "email", () => reader["email"].ToString()),
                LastUpdate = GetSafeValue(reader, "last_update", () => Convert.ToDateTime(reader["last_update"]))
            };
        }

        public Group MapGroup(MySqlDataReader reader)
        {
            return new Group()
            {
                Id = GetSafeValue(reader, "id", () => reader.GetInt32("id")),
                Name = GetSafeValue(reader, "name", () => reader["name"].ToString()),
                Password = GetSafeValue(reader, "password", () => reader["password"].ToString()),
                Description = GetSafeValue(reader, "description", () => reader["description"].ToString()),
                LastUpdate = GetSafeValue(reader, "last_update", () => Convert.ToDateTime(reader["last_update"]))
            };
        }

        public Subject MapSubject(MySqlDataReader reader)
        {
            return new Subject()
            {
                Id = GetSafeValue(reader, "id", () => reader.GetInt32("id")),
                Name = GetSafeValue(reader, "name", () => reader["name"].ToString()),
                Description = GetSafeValue(reader, "description", () => reader["description"].ToString()),
                LastUpdate = GetSafeValue(reader, "last_update", () => Convert.ToDateTime(reader["last_update"]))
            };
        }

        public SubjectGroup MapSubjectGroup(MySqlDataReader reader)
        {
            return new SubjectGroup()
            {
                Id = GetSafeValue(reader, "id", () => reader.GetInt32("id")),
                Place = GetSafeValue(reader, "place", () => reader["place"].ToString()),
                Teacher = GetSafeValue(reader, "teacher", () => reader["teacher"].ToString()),
                Description = GetSafeValue(reader, "description", () => reader["description"].ToString()),
                LastUpdate = GetSafeValue(reader, "last_update", () => Convert.ToDateTime(reader["last_update"]))
            };
        }

        public GroupEvent MapGroupEvent(MySqlDataReader reader)
        {
            return new GroupEvent()
            {
                Id = GetSafeValue(reader, "id", () => reader.GetInt32("id")),
                Name = GetSafeValue(reader, "name", () => reader["name"].ToString()),
                Description = GetSafeValue(reader, "description", () => reader["description"].ToString()),
                Date = GetSafeValue(reader, "date", () => reader["date"].ToString().ToDateTimeOrNull()),
                Place = GetSafeValue(reader, "place", () => reader["place"].ToString()),
                LastUpdate = GetSafeValue(reader, "last_update", () => Convert.ToDateTime(reader["last_update"])),
                ParentId = GetSafeValue(reader, "parent_id", () => reader.GetInt32("parent_id"))
            };
        }

        public SubjectEvent MapSubjectEvent(MySqlDataReader reader)
        {
            return new SubjectEvent()
            {
                Id = GetSafeValue(reader, "id", () => reader.GetInt32("id")),
                Name = GetSafeValue(reader, "name", () => reader["name"].ToString()),
                Description = GetSafeValue(reader, "description", () => reader["description"].ToString()),
                Date = GetSafeValue(reader, "date", () => reader["date"].ToString().ToDateTimeOrNull()),
                Place = GetSafeValue(reader, "place", () => reader["place"].ToString()),
                LastUpdate = GetSafeValue(reader, "last_update", () => Convert.ToDateTime(reader["last_update"])),
                ParentId = GetSafeValue(reader, "parent_id", () => reader.GetInt32("parent_id"))
            };
        }

        public SubjectGroupEvent MapSubjectGroupEvent(MySqlDataReader reader)
        {
            return new SubjectGroupEvent()
            {
                Id = GetSafeValue(reader, "id", () => reader.GetInt32("id")),
                Name = GetSafeValue(reader, "name", () => reader["name"].ToString()),
                Description = GetSafeValue(reader, "description", () => reader["description"].ToString()),
                Date = GetSafeValue(reader, "date", () => reader["date"].ToString().ToDateTimeOrNull()),
                Place = GetSafeValue(reader, "place", () => reader["place"].ToString()),
                LastUpdate = GetSafeValue(reader, "last_update", () => Convert.ToDateTime(reader["last_update"])),
                ParentId = GetSafeValue(reader, "parent_id", () => reader.GetInt32("parent_id"))
            };
        }

        private ICollection<T> MapMySqlList<T>(MySqlDataReader reader, Func<T> itemMapper)
        {
            List<T> result = new List<T>();
            while (reader.Read())
            {
                result.Add(itemMapper.Invoke());
            }
            return result;
        }

        private T GetSafeValue<T>(MySqlDataReader reader, string name, Func<T> function)
        {
            if (reader[name] != DBNull.Value)
            {
                return function.Invoke();
            }
            return default(T);
        }
    }

    public static class ConventerUtils
    {
        public static readonly string EMPTY_DATE_PATTERN = "0001-01-01 00:00:00";
        public static DateTime? ToDateTimeOrNull(this string str)
        {
            if (str == null || str == "" || str == EMPTY_DATE_PATTERN)
            {
                return null;
            }
            else
            {
                return Convert.ToDateTime(str);
            }
        }
    }
}
