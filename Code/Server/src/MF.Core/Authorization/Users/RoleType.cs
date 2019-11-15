using System;
using System.Collections.Generic;
using System.Text;

namespace MF.Authorization.Users
{
    [Flags]
    public enum RoleType
    {
        学生 = 1,
        家长 = 2,
        导师 = 4,
        管理员 = 8,
    }

    public static class RoleTypeExpansion
    {
        public static List<RoleType> ToList(this RoleType roleType)
        {
            var roleTypes = new List<RoleType>();
            foreach (var item in Enum.GetValues(typeof(RoleType)))
            {
                var _item = (RoleType)item;
                if (roleType.HasFlag(_item))
                {
                    roleTypes.Add(_item);
                }
            }
            return roleTypes;
        }

        public static RoleType ToOne(this IEnumerable<RoleType> roleTypes)
        {
            RoleType roleType = 0;
            if (roleTypes != null)
            {
                foreach (var item in roleTypes)
                {
                    roleType |= item;
                }
            }
            return  roleType;

        }
    }
}
