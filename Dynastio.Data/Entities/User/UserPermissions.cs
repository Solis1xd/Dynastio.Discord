using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Data
{
    public class UserPermissions
    {
        public List<UserPermission> Permissions { get; set; } = new List<UserPermission>() { UserPermission.Member };
        public bool HasPermission(UserPermission Permission)
        {
            return Permissions.Contains(Permission);
        }
        public bool HasPermissions(params UserPermission[] Permissions)
        {
            foreach (var p in Permissions)
            {
                if (!HasPermission(p))
                    return false;
            }
            return true;
        }
        public bool HasOnePermission(params UserPermission[] Permissions)
        {
            foreach (var p in Permissions)
            {
                if (HasPermission(p))
                    return true;
            }
            return false;
        }
        public void AddPermission(UserPermission Permission)
        {
            Permissions.Add(Permission);
        }
        public void RemovePermission(UserPermission Permission)
        {
            Permissions.Remove(Permission);
        }
    }
}
