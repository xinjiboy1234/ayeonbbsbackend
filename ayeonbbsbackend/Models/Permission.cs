using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Models
{
    public class Permission
    {
        public int PermissionId { get; set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public UserRole UserRole { get; set; }
        /// <summary>
        /// 板块信息
        /// </summary>
        public List<SecondCategory> SecondCategories { get; set; }
    }
}
