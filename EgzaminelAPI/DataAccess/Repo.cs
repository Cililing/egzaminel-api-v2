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
        bool RemoveToken(string token);
        bool RemoveToken(int userId);

        // TODO implement section
        // Users
        int? GetUserId(string username, string password);
        UserCredentials GetUserCredentials(string username);
        User GetUser(int id);
        ICollection<User> GetUsers(IEnumerable<int> ids);
        ApiResponse AddUser(User user);
        ApiResponse EditUser(User user);
        ApiResponse RemoveUser(int userId);
        IEnumerable<GroupPermission> GetGroupPermission(int userId);
        IEnumerable<SubjectGroupPermission> GetSubjectGroupPermission(int userId);
        ApiResponse CreartePermission(Permission permission);
        ApiResponse EditPermission(Permission permission);
        ApiResponse RemovePermission(Permission permission);

        // Groups
        Group GetGroup(int id);
        ICollection<Group> GetGroups(User user);
        ApiResponse AddGroup(Group group, int ownerId);
        ApiResponse EditGroup(Group group);
        ApiResponse RemoveGroup(Group group);

        // Subject
        int GetSubjectParentId(int subjectId);
        Subject GetSubject(int id);
        ICollection<Subject> GetSubjects(int parentId);
        ApiResponse AddSubject(Subject subject);
        ApiResponse EditSubject(Subject subject);
        ApiResponse RemoveSubject(Subject subject);

        // Subject groups
        int GetSubjectGroupGroupId(int subjectGroupId);
        SubjectGroup GetSubjectGroup(int id);
        ICollection<SubjectGroup> GetSubjectGroups(int parentId);
        ApiResponse AddSubjectGroup(SubjectGroup subjectGroup);
        ApiResponse EditSubjectGroup(SubjectGroup subjectGroup);
        ApiResponse RemoveSubjectGroup(SubjectGroup subjectGroup);

        // Events
        int GetEventParentId(Event eventObject);
        GroupEvent GetGroupEvent(int id);
        SubjectEvent GetSubjectEvent(int id);
        SubjectGroupEvent GetSubjectGroupEvent(int id);
        ICollection<GroupEvent> GetGroupEvents(int parentId);
        ICollection<SubjectEvent> GetSubjectEvents(int idparentId);
        ICollection<SubjectGroupEvent> GetSubjectGroupEvents(int parentId);
        ApiResponse AddEvent(Event eventObject);
        ApiResponse EditEvent(Event eventObject);
        ApiResponse RemoveEvent(Event eventObject);
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

        /// <summary>
        ///     Returns TokenModel by token 
        /// </summary>
        /// <param name="token">user token</param>
        /// <returns>
        ///     TokenModel Object
        /// </returns>
        public TokenModel GetToken(string token)
        {
            var query = String.Format(@"SELECT * FROM users_token WHERE auth_token = '{0}'", token);
            var result = GetItem(query, (reader) => _mapper.MapToken(reader));

            return result;
        }

        /// <summary>
        ///     Saves token in database
        /// </summary>
        /// <param name="token">TokenModel object</param>
        /// <returns>
        ///     True if saved, otherwise false
        /// </returns>
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

        /// <summary>
        ///     Updates Token in database.
        /// </summary>
        /// <param name="token">TokenModel object</param>
        /// <returns>
        ///     True if updated, otherwise false
        /// </returns>
        public bool UpdateToken(TokenModel token)
        {
            var query = String.Format(
                @"UPDATE users_token SET `issued_on` = {0}, `expires_on` = {1} WHERE auth_token = '{2}'",
                    PutSafeValue(token.IssuedOn, (x) => ConvertDate(x)),
                    PutSafeValue(token.ExpiresOn, (x) => ConvertDate(x)),
                    PutSafeValue(token.AuthToken, (x) => x.ToString()));

            return EditItem(query, (code) => code > 0);
        }

        /// <summary>
        ///     Removes Token by user token
        /// </summary>
        /// <param name="token">User token</param>
        /// <returns>
        ///     True if removed, otherwise false
        /// </returns>
        public bool RemoveToken(string token)
        {
            var query = String.Format(
                @"DELETE FROM users_token WHERE `auth_token` = {1}",
                PutSafeValue(token, (x) => x));

            return EditItem(query, (code) => code > 0);
        }

        /// <summary>
        ///     Removes Token by userId
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>
        ///     True if removed, otherwise false
        /// </returns>
        public bool RemoveToken(int userId)
        {
            var query = String.Format(
                @"DELETE FROM users_token WHERE `user_id` = {1}",
                PutSafeValue(userId, (x) => x.ToString()));

            return EditItem(query, (code) => code > 0);
        }

        #endregion

        #region USERS
        /// <summary>
        ///     Returns user id
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int? GetUserId(string username, string password)
        {
            var query = String.Format(@"SELECT id FROM users WHERE username = '{0}' AND password = '{1}'", username, password);
            var result = GetItem(query, (reader) => _mapper.MapUserId(reader));

            return result;
        }

        public UserCredentials GetUserCredentials(string username)
        {
            var query = String.Format(@"SELECT password, salt FROM users WHERE username = '{0}'", username);
            var result = GetItem(query, (reader) => _mapper.MapUserCredentials(reader));

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

        /// <summary>
        ///     Returns collection of users by theri ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ICollection<User> GetUsers(IEnumerable<int> ids)
        {
            var queryIds = string.Join(",", ids.ToArray());
            var query = String.Format(@"SELECT * FROM users WHERE id IN ({0})", queryIds);

            return GetCollection(query, (reader) => _mapper.MapUsers(reader));
        }

        public ApiResponse AddUser(User user)
        {
            var query = String.Format(
                @"INSERT INTO `users` (`id`, `username`, `password`, `salt`, `email`, `last_update`)
                VALUES (NULL, '{0}', '{1}', '{2}', '{3}', NULL); SELECT LAST_INSERT_ID()",
                PutSafeValue(user.Username, (x) => x.ToString()),
                PutSafeValue(user.EncryptedPassword, (x) => x.ToString()),
                PutSafeValue(user.Salt, (x) => x.ToString()),
                PutSafeValue(user.Email, (x) => x.ToString()));

            return AddItem(query, (id) => new ApiResponse()
            {
                IsSuccess = id >= 0,
                ResultCode = id
            });
        }

        public ApiResponse ChangePassword(User user)
        {
            var query = String.Format(
            @"UPDATE groups SET `password` = '{0}', `salt` = '{1} WHERE `users`.`id` = {2}",
            PutSafeValue(user.EncryptedPassword, (x) => x.ToString()),
            PutSafeValue(user.Salt, (x) => x.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse EditUser(User user)
        {
            var query = String.Format(
            @"UPDATE groups SET `email` = '{0}' WHERE `users`.`id` = {1}",
            PutSafeValue(user.Email, (x) => x.ToString()),
            PutSafeValue(user.Id, (x) => x.ToString()));

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        public ApiResponse RemoveUser(int userId)
        {
            var query = String.Format(@"DELETE FROM 'users' WHERE users.`id` = {0}", userId);

            return EditItem(query, (code) => new ApiResponse()
            {
                IsSuccess = code > 0,
                ResultCode = code
            });
        }

        /// <summary>
        ///     Returns collection of user permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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

        public ApiResponse RemovePermission(Permission permission)
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
            var events = GetGroupEvents(groupId.Value);
            result.Events = events;
            result.Subjects = GetSubjects(groupId.Value);

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
                var events = GetGroupEvents(groupId.Value);
                group.Events = events;
                group.Subjects = GetSubjects(groupId.Value);
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

        public ApiResponse RemoveGroup(Group group)
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
                PutSafeValue(subject.ParentGroup.Id, (x) => x.ToString()));

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

        public ApiResponse RemoveSubject(Subject subject)
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

        public ApiResponse RemoveSubjectGroup(SubjectGroup subjectGroup)
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

        public int GetEventParentId(Event eventObject)
        {
            var tableName = GetEventTableName(eventObject);
            var query = String.Format(@"SELECT parent_id FROM {0} WHERE id = {1}", tableName, eventObject.Id);

            var result = GetItem(query, (reader) => _mapper.MapEventParentId(reader));

            return result == null ? -1 : result.Value;
        }

       /// <summary>
       ///  Returns groupEvent object
       /// </summary>
       /// <param name="id">GroupEvent id</param>
       /// <returns></returns>
        public GroupEvent GetGroupEvent(int id)
        {
            var query = String.Format(@"SELECT * FROM events_groups WHERE id = {0}", id);
            return GetItem(query, (reader) => _mapper.MapGroupEvent(reader));
        }

        public SubjectEvent GetSubjectEvent(int id)
        {
            var query = String.Format(@"SELECT * FROM events_subjects WHERE id = {0}", id);
            return GetItem(query, (reader) => _mapper.MapSubjectEvent(reader));
        }

        public SubjectGroupEvent GetSubjectGroupEvent(int id)
        {
            var query = String.Format(@"SELECT * FROM events_subjects_groups WHERE id = {0}", id);
            return GetItem(query, (reader) => _mapper.MapSubjectGroupEvent(reader));
        }

        /// <summary>
        ///     Returns groupEvents Collection
        /// </summary>
        /// <param name="parentId">GroupId</param>
        /// <returns></returns>
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

        public ApiResponse RemoveEvent(Event eventObject)
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
        /// <summary>
        ///     Returns null-safe value (val can be inserted safly to database)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Value</param>
        /// <param name="mapper">Mapper - ex. eventObject.Place, (x) => x.ToString()</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Convert date to string wich can be safly put into db
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private string ConvertDate(DateTime? dateTime)
        {
            return dateTime == null ? "NULL" : "'" + dateTime?.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        }

        /// <summary>
        ///     Return eventTabble name - can be used for every type of events
        /// </summary>
        /// <param name="eventObject">event instance</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Method open databaase and execute "INSERT INTO ..." Sql Command
        /// </summary>
        /// <typeparam name="T">mapper output</typeparam>
        /// <param name="sqlQuery">full sql query</param>
        /// <param name="mapper">Mapper for result (input: id of inserted item)</param>
        /// <returns>
        ///     Result of mapper
        /// </returns>
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

        /// <summary>
        ///     Opens database and edit item ("UPDATE")
        /// </summary>
        /// <typeparam name="T">mapper output</typeparam>
        /// <param name="sqlQuery">full sql squery </param>
        /// <param name="mapper">mapper (input: number of edited items)</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Returns single item from database ("SELECT")
        /// </summary>
        /// <typeparam name="T">mapper output</typeparam>
        /// <param name="sqlQuery">sqlQuery</param>
        /// <param name="mapper">mapper: (input: MySqlDataReader object executing sqlQuery)</param>
        /// <returns></returns>
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


        /// <summary>
        ///     Returns collection of items ("SELECT")
        /// </summary>
        /// <typeparam name="T">Type of items in output mapper collection</typeparam>
        /// <param name="sqlQuery">sql query</param>
        /// <param name="mapper">mapper: (input: MySqlDataReader object executing sqlQuery)</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Returns enumerable list of items ("SELECT")
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlQuery">sql query</param>
        /// <param name="mapper">mapper (input: MySqlDataReader object executing sqlQuery)</param>
        /// <returns></returns>
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

        /// <summary>
        ///  Simple ForEach function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        private void ForEach<T>(ICollection<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        #endregion

        #region NOT_IMPLEMENTED
        #endregion
    }
}