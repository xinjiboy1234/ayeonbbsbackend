using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Models
{
    public class PostInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public UserInfo Author { get; set; }
        public SecondCategory SecondCategory { get; set; }
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 状态 1 为发布，100 为审核， 2为 管理员删除， 3为用户删除
        /// </summary>
        public int Status { get; set; }
        public List<PostGood> PostGoods { get; set; }
        public List<ReplyInfo> ReplyInfos { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }
        public string Description { get; set; }// 概述
        /// <summary>
        /// 是否置顶
        /// </summary>
        public int IsTop { get; set; } = 999;
        public int Watch { get; set; }
    }
}
