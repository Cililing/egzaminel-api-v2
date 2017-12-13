using EgzaminelAPI.Models;
using System;
using System.Collections.Generic;

namespace EgzaminelAPI.Helpers
{
    public class DAOUtils
    {
        private static Dictionary<Type, int> eventTypeDictionary = new Dictionary<Type, int>
        {
            { typeof(GroupEvent), 1 },
            { typeof(SubjectEvent), 2 },
            { typeof(SubjectGroupEvent), 3 }
        };

        private static Dictionary<Type, int> permissionTypeDictionary = new Dictionary<Type, int>
        {
            { typeof(GroupPermission), 1 },
            { typeof(SubjectGroupPermission), 2 },
        };

        public static T DoByEventType<T>(Event eventObj, Func<T> groupAction, Func<T> subjectAction, Func<T> subjectGroupAction)
        {
            if (eventObj == null) return default(T);
            switch (eventTypeDictionary[eventObj.GetType()])
            {
                case 1:
                    if (groupAction != null) return groupAction.Invoke();
                    break;
                case 2:
                    if (subjectAction != null) return subjectAction.Invoke();
                    break;
                case 3:
                    if (subjectGroupAction != null) return subjectGroupAction.Invoke();
                    break;
                default:
                    return default(T);
            }
            return default(T);
        }

        public static T DoByPermissionType<T>(Permission permission, Func<T> groupPermissionAction, Func<T> subjectGroupPermissionAction)
        {
            if (permission == null) return default(T);
            switch (permissionTypeDictionary[permission.GetType()])
            {
                case 1:
                    if (groupPermissionAction != null) return groupPermissionAction.Invoke();
                    break;
                case 2:
                    if (subjectGroupPermissionAction != null) return subjectGroupPermissionAction.Invoke();
                    break;
                default:
                    return default(T);
            }

            return default(T);
        }
    }
}
