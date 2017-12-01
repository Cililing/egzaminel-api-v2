using EgzaminelAPI.Auth;
using EgzaminelAPI.Context;
using EgzaminelAPI.Helpers;
using EgzaminelAPI.Models;
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
        bool RemoveToken(string token);
        bool RemoveTokenByUserId(int userId);

        // TODO implement section
        // Users
        int? GetUserId(string username, string password);
        User GetUser(int id);
        ICollection<User> GetUsers(IEnumerable<int> ids);
        ApiResponse PostUser(User user);
        ApiResponse UpdateUser(User user);
        ApiResponse DeleteUser(User user);
        IEnumerable<GroupPermission> GetGroupPermission(int userId);
        IEnumerable<SubjectGroupPermission> GetSubjectGroupPermission(int userId);
        ApiResponse PostPermission(Permission permission);
        ApiResponse UpdatePermission(Permission permission);
        ApiResponse RemovePermission(Permission permission);

        // Groups
        Group GetGroup(int id); // implemented
        ICollection<Group> GetGroups(User user);
        ApiResponse PostGroup(Group group, int ownerId);
        ApiResponse UpdateGroup(Group group);
        ApiResponse DeleteGroup(Group group);

        // TODO implement section
        // Subject
        Subject GetSubject(int id);
        ICollection<Subject> GetSubjects(int parentId);
        ApiResponse PostSubject(Subject subject);
        ApiResponse UpdateSubject(Subject subject);
        ApiResponse DeleteSubject(Subject subject);

        // TODO implement section
        // Subject groups
        int GetSubjectParentId(int subjectId);
        SubjectGroup GetSubjectGroup(int id);
        ICollection<SubjectGroup> GetSubjectGroups(int parentId); // implemented
        ApiResponse PostSubjectGroup(SubjectGroup subjectGroup);
        ApiResponse UpdateSubjectGroup(SubjectGroup subjectGroup);
        ApiResponse DeleteSubjectGroup(SubjectGroup subjectGroup);

        // Events
        GroupEvent GetGroupEvent(int id);
        SubjectEvent GetSubjectEvent(int id);
        SubjectGroupEvent GetSubjectGroupEvent(int id);
        ICollection<GroupEvent> GetGroupEvents(int parentId);
        ICollection<SubjectEvent> GetSubjectEvents(int idparentId);
        ICollection<SubjectGroupEvent> GetSubjectGroupEvents(int parentId);
        ApiResponse PostEvent(Event eventObject);
        ApiResponse UpdateEvent(Event eventObject);
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
                PutSafeValue(token.AuthToken, () => token.AuthToken.ToString()),
                PutSafeValue(token.UserId, () => token.UserId.ToString()),
                PutSafeValue(token.IssuedOn, () => ConvertDate(token.IssuedOn)),
                PutSafeValue(token.ExpiresOn, () => ConvertDate(token.ExpiresOn)));

            return EditItem(query, (code) => code > 0);
        }

        public bool UpdateToken(TokenModel token)
        {
            var query = String.Format(
                @"UPDATE users_token SET `issued_on` = {0}, `expires_on` = {1} WHERE auth_token = '{2}'",
                    PutSafeValue(token.IssuedOn, () => ConvertDate(token.IssuedOn)),
                    PutSafeValue(token.ExpiresOn, () => ConvertDate(token.ExpiresOn)),
                    PutSafeValue(token.AuthToken, () => token.AuthToken.ToString()));

            return EditItem(query, (code) => code > 0);
        }

        public bool RemoveToken(string token)
        {
            var query = String.Format(
                @"DELETE FROM users_token WHERE `auth_token` = {1}",
                PutSafeValue(token, () => token));

            return EditItem(query, (code) => code > 0);
        }

        public bool RemoveTokenByUserId(int userId)
        {
            var query = String.Format(
                @"DELETE FROM users_token WHERE `user_id` = {1}",
                PutSafeValue(userId, () => userId.ToString()));

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

        public ApiResponse PostPermission(Permission permission)
        {
            var tableName = GetPermissionTableName(permission);

            var query = String.Format(@"INSERT INTO `{0}` (`user_id`, `object_id`, `has_admin_permission`, `can_modify`, `last_update`)
                                    VALUES ('{1}', '{2}', '{3}', '{4}', NULL)",
                                    tableName,
                                    PutSafeValue(permission.UserId, () => permission.UserId.ToString()),
                                    PutSafeValue(permission.ObjectId, () => permission.ObjectId.ToString()),
                                    PutSafeValue(permission.HasAdminPermission, () => (Convert.ToInt32(permission.HasAdminPermission).ToString())),
                                    PutSafeValue(permission.HasAdminPermission, () => (Convert.ToInt32(permission.CanModify).ToString())));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse UpdatePermission(Permission permission)
        {
            var tableName = GetPermissionTableName(permission);
            var query = String.Format(@"UPDATE `{0}` SET `user_id` = '{1}', `object_id` = '{2}', `has_admin_permission` = '{3}', `can_modify` = '{4}'
                                    WHERE `{0}`.user_id = '{1}' AND `{0}`.object_id = '{2}'",
                                    tableName,
                                    PutSafeValue(permission.UserId, () => permission.UserId.ToString()),
                                    PutSafeValue(permission.ObjectId, () => permission.ObjectId.ToString()),
                                    PutSafeValue(permission.HasAdminPermission, () => (Convert.ToInt32(permission.HasAdminPermission).ToString())),
                                    PutSafeValue(permission.HasAdminPermission, () => (Convert.ToInt32(permission.CanModify).ToString())));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse RemovePermission(Permission permission)
        {
            var tableName = GetPermissionTableName(permission);
            var query = String.Format(@"DELETE FROM `{0}` WHERE `{0}.`user_id` = {1} AND `{0}.`object_id` = {2}",
                tableName,
                PutSafeValue(permission.UserId, () => permission.UserId.ToString()),
                PutSafeValue(permission.ObjectId, () => permission.ObjectId.ToString()));

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

        public ApiResponse PostGroup(Group group, int ownerId)
        {
            var query = String.Format(
                @"INSERT INTO `groups` (`id`, `name`, `description`, `password`, `owner`, `last_update`)
                VALUES (NULL, '{0}', '{1}', '{2}', '{3}', NULL); SELECT LAST_INSERT_ID()",
                PutSafeValue(group.Name, () => group.Name.ToString()),
                PutSafeValue(group.Description, () => group.Description.ToString()),
                PutSafeValue(group.Password, () => group.Password.ToString()),
                PutSafeValue(ownerId, () => ownerId.ToString()));

            return AddItem(query, (id) => new ApiResponse()
            {
                IsSuccess = id >= 0,
                ResultCode = id
            });
        }

        public ApiResponse UpdateGroup(Group group)
        {
            var query = String.Format(
                @"UPDATE groups SET `name` = '{0}', `description` = '{1}', `password` = '{2}' WHERE `groups`.`id` = {3}",
                PutSafeValue(group.Name, () => group.Name.ToString()),
                PutSafeValue(group.Description, () => group.Description.ToString()),
                PutSafeValue(group.Password, () => group.Password.ToString()),
                PutSafeValue(group.Id, () => group.Id.ToString()));

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

        public ApiResponse PostSubject(Subject subject)
        {
            var query = String.Format(
                @"INSERT INTO subjects (id, name, description, group_id, last_update)
                VALUES (NULL, '{0}', '{1}', '{2}', NULL); SELECT LAST_INSERT_ID()",
                PutSafeValue(subject.Name, () => subject.Name.ToString()),
                PutSafeValue(subject.Description, () => subject.Description.ToString()),
                PutSafeValue(subject.ParentGroup, () => subject.ParentGroup.Id.ToString()));

            return AddItem(query, (id) => new ApiResponse()
            {
                IsSuccess = id >= 0,
                ResultCode = id
            });
        }

        public ApiResponse UpdateSubject(Subject subject)
        {
            var query = String.Format(
                @"UPDATE subjects SET name = '{0}', description = '{1}' WHERE subjects.id = {2}",
                PutSafeValue(subject.Name, () => subject.Name.ToString()),
                PutSafeValue(subject.Description, () => subject.Description.ToString()),
                PutSafeValue(subject.Id, () => subject.Id.ToString()));

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

        public ICollection<SubjectGroup> GetSubjectGroups(int subjectId)
        {
            var query = String.Format(@"SELECT * FROM subject_groups WHERE subject_id = {0}", subjectId);
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

        public ApiResponse PostEvent(Event eventObject)
        {
            string tableName = GetEventTableName(eventObject);

            var query = String.Format(
                @"INSERT INTO `{0}`
                (`id`, `name`, `description`, `date`, `place`, `parent_id`, `last_update`)
                VALUES (NULL, '{1}', '{2}', {3}, '{4}', '{5}', NULL)",
                tableName,
                PutSafeValue(eventObject.Name, () => eventObject.Name.ToString()),
                PutSafeValue(eventObject.Description, () => eventObject.Description.ToString()),
                PutSafeValue(eventObject, () => ConvertDate(eventObject.Date)),
                PutSafeValue(eventObject, () => eventObject.Place.ToString()),
                PutSafeValue(eventObject, () => eventObject.ParentId.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse UpdateEvent(Event eventObject)
        {
            string tableName = GetEventTableName(eventObject);

            // 1, 2, 3, 4 - fields
            // 5 - id
            var query = String.Format(
                @"UPDATE `{0}` SET `name` = '{1}', `description` = '{2}', `date` = {3}, `place` = '{4}' WHERE `{0}`.`id` = {5}",
                tableName,
                PutSafeValue(eventObject.Name, () => eventObject.Name.ToString()),
                PutSafeValue(eventObject.Description, () => eventObject.Description.ToString()),
                PutSafeValue(eventObject.Date, () => ConvertDate(eventObject.Date)),
                PutSafeValue(eventObject.Place, () => eventObject.Place.ToString()),
                PutSafeValue(eventObject.Id, () => eventObject.Id.ToString()));

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
                PutSafeValue(eventObject.Id, () => eventObject.Id.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        #endregion

        #region HELPERS        

        private string PutSafeValue<T>(T value, Func<string> mapper)
        {
            if (value == null || value.ToString() == "")
            {
                return "NULL";
                //return DBNull.Value.ToString();
            }
            else
            {
                return mapper.Invoke();
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

        public ApiResponse PostUser(User user)
        {
            throw new NotImplementedException();
        }

        public ApiResponse UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public ApiResponse DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public SubjectGroup GetSubjectGroup(int id)
        {
            throw new NotImplementedException();
        }

        public ApiResponse PostSubjectGroup(SubjectGroup subjectGroup)
        {
            throw new NotImplementedException();
        }

        public ApiResponse UpdateSubjectGroup(SubjectGroup subjectGroup)
        {
            throw new NotImplementedException();
        }

        public ApiResponse DeleteSubjectGroup(SubjectGroup subjectGroup)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}