using ayeonbbsbackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.ViewModels
{
    public class UserInfoViewModel: UserInfo
    {
        public int PostManageCount { get; set; }
        public int PublishCategoryCount { get; set; }
        public int PostCount { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public List<int> UserIds { get; set; } // 用户ID集合，用于批量删除
        public bool IsPostManager { get; set; }
    }
}
