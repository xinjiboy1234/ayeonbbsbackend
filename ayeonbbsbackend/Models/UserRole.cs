using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Models
{
    public class UserRole
    {
        public int UserRoleId { get; set; }
        public RoleInfo RoleInfo { get; set; }
        public List<UserInfo> UserInfos { get; set; }
    }
}
