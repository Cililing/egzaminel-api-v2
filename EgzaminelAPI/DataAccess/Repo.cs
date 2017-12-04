using EgzaminelAPI.Auth;
using EgzaminelAPI.Context;
using EgzaminelAPI.Helpers;
using EgzaminelAPI.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EgzaminelAPI.DataAccess
{
    public interface IRepo
    {
        // TOKENS
        TokenModel GetToken(string token);
        bool SaveToken(TokenModel token);
        bool UpdateToken(TokenModel token);
        bool DeleteToken(string token);
        bool DeleteTokenByUserId(int userId);

        // TODO implement section
        // Users
        int? GetUserId(string username, string password);
        User GetUser(int id);
        ICollection<User> GetUsers(IEnumerable<int> ids);
        ApiResponse AddUser(User user);
        ApiResponse EditUser(User user);
        ApiResponse DeleteUser(User user);
        IEnumerable<GroupPermission> GetGroupPermission(int userId);
        IEnumerable<SubjectGroupPermission> GetSubjectGroupPermission(int userId);
        ApiResponse CreartePermission(Permission permission);
        ApiResponse EditPermission(Permission permission);
        ApiResponse DeletePermission(Permission permission);

        // Groups
        Group GetGroup(int id);
        ICollection<Group> GetGroups(User user);
        ApiResponse AddGroup(Group group, int ownerId);
        ApiResponse EditGroup(Group group);
        ApiResponse DeleteGroup(Group group);

        // Subject
        int GetSubjectParentId(int subjectId);
        Subject GetSubject(int id);
        ICollection<Subject> GetSubjects(int parentId);
        ApiResponse AddSubject(Subject subject);
        ApiResponse EditSubject(Subject subject);
        ApiResponse DeleteSubject(Subject subject);

        // TODO implement section
        // Subject groups
        int GetSubjectGroupGroupId(int subjectGroupId);
        SubjectGroup GetSubjectGroup(int id);
        ICollection<SubjectGroup> GetSubjectGroups(int parentId); // implemented
        ApiResponse AddSubjectGroup(SubjectGroup subjectGroup);
        ApiResponse EditSubjectGroup(SubjectGroup subjectGroup);
        ApiResponse DeleteSubjectGroup(SubjectGroup subjectGroup);

        // Events
        GroupEvent GetGroupEvent(int id);
        SubjectEvent GetSubjectEvent(int id);
        SubjectGroupEvent GetSubjectGroupEvent(int id);
        ICollection<GroupEvent> GetGroupEvents(int parentId);
        ICollection<SubjectEvent> GetSubjectEvents(int idparentId);
        ICollection<SubjectGroupEvent> GetSubjectGroupEvents(int parentId);
        ApiResponse AddEvent(Event eventObject);
        ApiResponse EditEvent(Event eventObject);
        ApiResponse DeleteEvent(Event eventObject);
    }

    public class Repo : IRepo
    {
        private readonly IEgzaminelContext _respositoryContext;
        private readonly IMapper _mapper;

        public Repo(IEgzaminelContext respositoryContext, IMapper mapper)
        {
            this._respositoryContext = respositoryContext;
            this._mapper = mapper;
        }

        #region TOKEN

        public TokenModel GetToken(string token)
        {
            var query = String.Format(@"SELECT * FROM users_token WHERE auth_token = '{0}'", token);
            var result = GetItem(query, (reader) => _mapper.MapToken(reader));

            return result;
        }

        public bool SaveToken(TokenModel token)
        {
            var query = String.Format(
                @"REPLACE INTO users_token
                (`auth_token`, `user_id`, `issued_on`,`expires_on`)
                VALUES ('{0}', '{1}', {2}, {3})",
                PutSafeValue(token.AuthToken, (x) => x.ToString()),
                PutSafeValue(token.UserId, (x) => x.ToString()),
                PutSafeValue(token.IssuedOn, (x) => ConvertDate(x)),
                PutSafeValue(token.ExpiresOn, (x) => ConvertDate(x)));

            return EditItem(query, (code) => code > 0);
        }

        public bool UpdateToken(TokenModel token)
        {
            var query = String.Format(
                @"UPDATE users_token SET `issued_on` = {0}, `expires_on` = {1} WHERE auth_token = '{2}'",
                    PutSafeValue(token.IssuedOn, (x) => ConvertDate(x)),
                    PutSafeValue(token.ExpiresOn, (x) => ConvertDate(x)),
                    PutSafeValue(token.AuthToken, (x) => x.ToString()));

            return EditItem(query, (code) => code > 0);
        }

        public bool DeleteToken(string token)
        {
            var query = String.Format(
                @"DELETE FROM users_token WHERE `auth_token` = {1}",
                PutSafeValue(token, (x) => x));

            return EditItem(query, (code) => code > 0);
        }

        public bool DeleteTokenByUserId(int userId)
        {
            var query = String.Format(
                @"DELETE FROM users_token WHERE `user_id` = {1}",
                PutSafeValue(userId, (x) => x.ToString()));

            return EditItem(query, (code) => code > 0);
        }

        #endregion

        #region USERS

        public int? GetUserId(string username, string password)
        {
            var query = String.Format(@"SELECT id FROM users WHERE username = '{0}' AND password = '{1}'", username, password);
            var result = GetItem(query, (reader) => _mapper.MapUserId(reader));

            return result;
        }

        public User GetUser(int id)
        {
            var query = String.Format(@"SELECT * FROM users WHERE id = '{0}'", id);
            var result = GetItem(query, (reader) => _mapper.MapUser(reader));

            if (result != null)
            {
                result.GroupsPermissions = GetGroupPermission(id);
                result.SubjectGroupsPermissions = GetSubjectGroupPermission(id);
            }

            return result;
        }

        public ICollection<User> GetUsers(IEnumerable<int> ids)
        {
            var queryIds = string.Join(",", ids.ToArray());
            var query = String.Format(@"SELECT * FROM users WHERE id IN ({0})", queryIds);

            return GetCollection(query, (reader) => _mapper.MapUsers(reader));
        }

        public IEnumerable<GroupPermission> GetGroupPermission(int userId)
        {
            var query = String.Format(@"SELECT * FROM groups_permissions WHERE user_id = '{0}'", userId);
            return GetList(query, (reader) => _mapper.MapPermissions<GroupPermission>(reader));
        }

        public IEnumerable<SubjectGroupPermission> GetSubjectGroupPermission(int userId)
        {
            var query = String.Format(@"SELECT * FROM subject_groups_permissions WHERE user_id = '{0}'", userId);
            return GetList(query, (reader) => _mapper.MapPermissions<SubjectGroupPermission>(reader));
        }

        public ApiResponse CreartePermission(Permission permission)
        {
            var tableName = GetPermissionTableName(permission);

            var query = String.Format(@"INSERT INTO `{0}` (`user_id`, `object_id`, `has_admin_permission`, `can_modify`, `last_update`)
                                    VALUES ('{1}', '{2}', '{3}', '{4}', NULL)",
                                    tableName,
                                    PutSafeValue(permission.UserId, (x) => x.ToString()),
                                    PutSafeValue(permission.ObjectId, (x) => x.ToString()),
                                    PutSafeValue(permission.HasAdminPermission, (x) => (Convert.ToInt32(x).ToString())),
                                    PutSafeValue(permission.CanModify, (x) => (Convert.ToInt32(x).ToString())));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse EditPermission(Permission permission)
        {
            var tableName = GetPermissionTableName(permission);
            var query = String.Format(@"UPDATE `{0}` SET `user_id` = '{1}', `object_id` = '{2}', `has_admin_permission` = '{3}', `can_modify` = '{4}'
                                    WHERE `{0}`.user_id = '{1}' AND `{0}`.object_id = '{2}'",
                                    tableName,
                                    PutSafeValue(permission.UserId, (x) => x.ToString()),
                                    PutSafeValue(permission.ObjectId, (x) => x.ToString()),
                                    PutSafeValue(permission.HasAdminPermission, (x) => (Convert.ToInt32(x).ToString())),
                                    PutSafeValue(permission.CanModify, (x) => (Convert.ToInt32(x).ToString())));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse DeletePermission(Permission permission)
        {
            var tableName = GetPermissionTableName(permission);
            var query = String.Format(@"DELETE FROM `{0}` WHERE `{0}.`user_id` = {1} AND `{0}.`object_id` = {2}",
                tableName,
                PutSafeValue(permission.UserId, (x) => x.ToString()),
                PutSafeValue(permission.ObjectId, (x) => x.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        #endregion

        #region GROUPS

        public Group GetGroup(int id)
        {
            var query = String.Format(@"SELECT * FROM groups WHERE id = {0}", id);
            var result = GetItem(query, (reader) => _mapper.MapGroup(reader));

            if (result == null)
            {
                return null;
            }

            var groupId = result.Id;
            var events = GetGroupEvents(groupId);
            result.Events = events;
            result.Subjects = GetSubjects(groupId);

            return result;
        }

        public ICollection<Group> GetGroups(User user)
        {
            if (!user.GroupsPermissions.Any()) return null;

            var ids = user.GroupsPermissions.Select(x => x.ObjectId);
            var queryIds = string.Join(",", ids.ToArray());
            var query = String.Format(@"SELECT * FROM groups WHERE id IN ({0})", queryIds);
            var result = GetCollection(query, (reader) => _mapper.MapGroups(reader));

            if (result == null) return null;

            ForEach(result, (group) =>
            {
                var groupId = group.Id;
                var events = GetGroupEvents(groupId);
                group.Events = events;
                group.Subjects = GetSubjects(groupId);
            });

            return result;
        }

        public ApiResponse AddGroup(Group group, int ownerId)
        {
            var query = String.Format(
                @"INSERT INTO `groups` (`id`, `name`, `description`, `password`, `owner`, `last_update`)
                VALUES (NULL, '{0}', '{1}', '{2}', '{3}', NULL); SELECT LAST_INSERT_ID()",
                PutSafeValue(group.Name, (x) => x.ToString()),
                PutSafeValue(group.Description, (x) => x.ToString()),
                PutSafeValue(group.Password, (x) => x.ToString()),
                PutSafeValue(ownerId, (x) => x.ToString()));

            return AddItem(query, (id) => new ApiResponse()
            {
                IsSuccess = id >= 0,
                ResultCode = id
            });
        }

        public ApiResponse EditGroup(Group group)
        {
            var query = String.Format(
                @"UPDATE groups SET `name` = '{0}', `description` = '{1}', `password` = '{2}' WHERE `groups`.`id` = {3}",
                PutSafeValue(group.Name, (x) => x.ToString()),
                PutSafeValue(group.Description, (x) => x.ToString()),
                PutSafeValue(group.Password, (x) => x.ToString()),
                PutSafeValue(group.Id, (x) => x.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse DeleteGroup(Group group)
        {
            var query = String.Format(@"DELETE FROM groups WHERE groups.`id` = {0}", group.Id);

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        #endregion

        #region SUBJECTS

        public int GetSubjectGroupGroupId(int subjectGroupId)
        {
            var query = String.Format(@"SELECT id FROM groups WHERE id = 
	                                    (SELECT group_id FROM subjects WHERE id =
     	                                (SELECT subject_id FROM subject_groups WHERE id = {0}))", subjectGroupId);
            var result = GetItem(query, (reader) => _mapper.MapSubjectGroupGroupId(reader));

            return result == null ? -1 : result.Value;
        }

        public int GetSubjectParentId(int subjectId)
        {
            var query = String.Format(@"SELECT group_id FROM subjects WHERE id = {0}", subjectId);
            var result = GetItem(query, (reader) => _mapper.MapSubjectParentId(reader));

            return result == null ? -1 : result.Value;
        }

        public ICollection<Subject> GetSubjects(int groupId)
        {
            var query = String.Format(@"SELECT * FROM subjects WHERE group_id = {0}", groupId);
            var result = GetCollection(query, (reader) => _mapper.MapSubjects(reader));

            if (result.Count == 0)
            {
                return null;
            }

            ForEach(result, (item) => item.Events = GetSubjectEvents(item.Id));
            ForEach(result, (item) => item.SubjectGroups = GetSubjectGroups(item.Id));

            return result;
        }

        public Subject GetSubject(int id)
        {
            var query = String.Format(@"SELECT * FROM subjects WHERE id = {0}", id);
            var result = GetItem(query, (reader) => _mapper.MapSubject(reader));

            if (result == null)
            {
                return null;
            }

            result.Events = GetSubjectEvents(result.Id);
            result.SubjectGroups = GetSubjectGroups(result.Id);

            return result;
        }

        public ApiResponse AddSubject(Subject subject)
        {
            var query = String.Format(
                @"INSERT INTO subjects (id, name, description, group_id, last_update)
                VALUES (NULL, '{0}', '{1}', '{2}', NULL); SELECT LAST_INSERT_ID()",
                PutSafeValue(subject.Name, (x) => x.ToString()),
                PutSafeValue(subject.Description, (x) => x.ToString()),
                PutSafeValue(subject.ParentGroup, (x) => x.ToString()));

            return AddItem(query, (id) => new ApiResponse()
            {
                IsSuccess = id >= 0,
                ResultCode = id
            });
        }

        public ApiResponse EditSubject(Subject subject)
        {
            var query = String.Format(
                @"UPDATE subjects SET name = '{0}', description = '{1}' WHERE subjects.id = {2}",
                PutSafeValue(subject.Name, (x) => x.ToString()),
                PutSafeValue(subject.Description, (x) => x.ToString()),
                PutSafeValue(subject.Id, (x) => x.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse DeleteSubject(Subject subject)
        {
            var query = String.Format(@"DELETE FROM subjects WHERE subjects.id = {0}", subject.Id);

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        #endregion

        #region SUBJECT GROUPS

        public SubjectGroup GetSubjectGroup(int id)
        {
            var query = String.Format(@"SELECT * FROM subject_groups WHERE id = {0}", id);
            var result = GetItem(query, (reader) => _mapper.MapSubjectGroup(reader));

            if (result == null)
            {
                return null;
            }

            result.Events = GetSubjectGroupEvents(result.Id);

            return result;
        }

        public ApiResponse AddSubjectGroup(SubjectGroup subjectGroup)
        {
            var query = String.Format(
                @"INSERT INTO subject_groups (id, subject_id, place, teacher, description, last_update)
                VALUES (NULL, '{0}', '{1}', '{2}', '{3}', NULL); SELECT LAST_INSERT_ID()",
                PutSafeValue(subjectGroup.ParentSubject.Id, (x) => x.ToString()),
                PutSafeValue(subjectGroup.Place, (x) => x.ToString()),
                PutSafeValue(subjectGroup.Teacher, (x) => x.ToString()),
                PutSafeValue(subjectGroup.Description, (x) => x.ToString()));

            return AddItem(query, (id) => new ApiResponse()
            {
                IsSuccess = id >= 0,
                ResultCode = id
            });
        }

        public ApiResponse EditSubjectGroup(SubjectGroup subjectGroup)
        {
            var query = String.Format(
                @"UPDATE subject_groups SET
                                place = '{0}',
                                teacher = '{1}',
                                description = '{2}'
                WHERE id = {3}",
                PutSafeValue(subjectGroup.Place, (x) => x.ToString()),
                PutSafeValue(subjectGroup.Teacher, (x) => x.ToString()),
                PutSafeValue(subjectGroup.Description, (x) => x.ToString()),
                PutSafeValue(subjectGroup.Id, (x) => x.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse DeleteSubjectGroup(SubjectGroup subjectGroup)
        {
            var query = String.Format(@"DELETE FROM subject_groups WHERE id = {0}", subjectGroup.Id);

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ICollection<SubjectGroup> GetSubjectGroups(int parentId)
        {
            var query = String.Format(@"SELECT * FROM subject_groups WHERE subject_id = {0}", parentId);
            var result = GetCollection(query, (reader) => _mapper.MapSubjectGroups(reader));
            
            if (result.Count == 0)
            {
                return null;
            }

            var events = GetSubjectGroupEvents(result.First().Id);
            ForEach(result, (item) => item.Events = events);

            return result;
        }

        #endregion

        #region EVENTS

        public GroupEvent GetGroupEvent(int id)
        {
            var query = String.Format(@"SELECT * FROM events_groups WHERE id = {0}", id);
            return GetItem(query, (reader) => _mapper.MapGroupEvent(reader));
        }

        public SubjectEvent GetSubjectEvent(int id)
        {
            var query = String.Format(@"SELECT * FROM events_groups WHERE id = {0}", id);
            return GetItem(query, (reader) => _mapper.MapSubjectEvent(reader));
        }

        public SubjectGroupEvent GetSubjectGroupEvent(int id)
        {
            var query = String.Format(@"SELECT * FROM events_groups WHERE id = {0}", id);
            return GetItem(query, (reader) => _mapper.MapSubjectGroupEvent(reader));
        }


        public ICollection<GroupEvent> GetGroupEvents(int parentId)
        {
            var query = String.Format(@"SELECT * FROM events_groups WHERE parent_id = {0}", parentId);
            return GetCollection(query, (reader) => _mapper.MapGroupEvents(reader));
        }

        public ICollection<SubjectEvent> GetSubjectEvents(int parentId)
        {
            var query = String.Format(@"SELECT * FROM events_subjects WHERE parent_id = {0}", parentId);
            return GetCollection(query, (reader) => _mapper.MapSubjectEvents(reader));
        }

        public ICollection<SubjectGroupEvent> GetSubjectGroupEvents(int parentId)
        {
            var query = String.Format(@"SELECT * FROM events_subjects_groups WHERE parent_id = {0}", parentId);
            return GetCollection(query, (reader) => _mapper.MapSubjectGroupEvents(reader));
        }

        public ApiResponse AddEvent(Event eventObject)
        {
            string tableName = GetEventTableName(eventObject);

            var query = String.Format(
                @"INSERT INTO `{0}`
                (`id`, `name`, `description`, `date`, `place`, `parent_id`, `last_update`)
                VALUES (NULL, '{1}', '{2}', {3}, '{4}', '{5}', NULL)",
                tableName,
                PutSafeValue(eventObject.Name, (x) => x.ToString()),
                PutSafeValue(eventObject.Description, (x) => x.ToString()),
                PutSafeValue(eventObject.Date, (x) => ConvertDate(x)),
                PutSafeValue(eventObject.Place, (x) => x.ToString()),
                PutSafeValue(eventObject.ParentId, (x) => x.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse EditEvent(Event eventObject)
        {
            string tableName = GetEventTableName(eventObject);

            // 1, 2, 3, 4 - fields
            // 5 - id
            var query = String.Format(
                @"UPDATE `{0}` SET `name` = '{1}', `description` = '{2}', `date` = {3}, `place` = '{4}' WHERE `{0}`.`id` = {5}",
                tableName,
                PutSafeValue(eventObject.Name, (x) => x.ToString()),
                PutSafeValue(eventObject.Description, (x) => x.ToString()),
                PutSafeValue(eventObject.Date, (x) => ConvertDate(x)),
                PutSafeValue(eventObject.Place, (x) => x.ToString()),
                PutSafeValue(eventObject.Id, (x) => x.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse DeleteEvent(Event eventObject)
        {
            string tableName = GetEventTableName(eventObject);

            // 0 - table name
            var query = String.Format(
                @"DELETE FROM `{0}` WHERE `{0}`.`id` = {1}",
                tableName,
                PutSafeValue(eventObject.Id, (x) => x.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        #endregion

        #region HELPERS        

        private string PutSafeValue<T>(T value, Func<T, string> mapper)
        {
            if (value == null || value.ToString() == "")
            {
                return "NULL";
                //return DBNull.Value.ToString();
            }
            else
            {
                return mapper.Invoke(value);
            }
        }

        private string ConvertDate(DateTime? dateTime)
        {
            return dateTime == null ? "NULL" : "'" + dateTime?.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        }

        private string GetEventTableName(Event eventObject)
        {
            return DAOUtils.DoByEventType<string>(eventObject,
                () => "events_groups",
                () => "events_subjects",
                () => "events_subjects_groups");
        }

        private string GetPermissionTableName(Permission permission)
        {
            return DAOUtils.DoByPermissionType(permission,
                () => "groups_permissions",
                () => "subject_groups_permissions");
        }

        private T AddItem<T>(string sqlQuery, Func<int, T> mapper)
        {
            using (MySqlConnection conn = _respositoryContext.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                return mapper.Invoke(id);
            }
        }

        private T EditItem<T>(string sqlQuery, Func<int, T> mapper)
        {
            using (MySqlConnection conn = _respositoryContext.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
                int resultCode = cmd.ExecuteNonQuery();
                return mapper.Invoke(resultCode);
            }
        }

        private T GetItem<T>(string sqlQuery, Func<MySqlDataReader, T> mapper)
        {
            using (MySqlConnection conn = _respositoryContext.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return mapper.Invoke(reader);
                    }

                    else return default(T);
                }
            }
        }

        private ICollection<T> GetCollection<T>(string sqlQuery, Func<MySqlDataReader, ICollection<T>> mapper)
        {
            using (MySqlConnection conn = _respositoryContext.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    return mapper.Invoke(reader);
                }
            }
        }

        private IEnumerable<T> GetList<T>(string sqlQuery, Func<MySqlDataReader, IEnumerable<T>> mapper)
        {
            using (MySqlConnection conn = _respositoryContext.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    return mapper.Invoke(reader);
                }
            }
        }

        private void ForEach<T>(ICollection<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        #endregion

        #region NOT_IMPLEMENTED

        public ApiResponse AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public ApiResponse EditUser(User user)
        {
            throw new NotImplementedException();
        }

        public ApiResponse DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}