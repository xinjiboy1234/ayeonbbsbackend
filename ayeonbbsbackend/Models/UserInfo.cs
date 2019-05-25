using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Models
{
    public class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public int IsSupperManager { get; set; }
        public int Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public List<PostManager> PostManagers { get; set; }
        public List<UserPublishCategory> UserPublishCategories { get; set; }
    }
}
