using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Models
{
    public class ReplyInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReplyId { get; set; }
        public string ReplyContent { get; set; }
        public DateTime? ReplyDate { get; set; }
        public UserInfo ReplyUser { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public int Floor { get; set; }
        /// <summary>
        /// 点赞
        /// </summary>
        public List<ReplyGood> ReplyGoods { get; set; }
        /// <summary>
        /// 第一级别ID
        /// </summary>
        public int ParentReplyId { get; set; }

        public PostInfo PostInfo { get; set; }

        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 被回复的ID
        /// </summary>
        public int? RepliedId { get; set; }
        public int? RepliedUserId { get; set; }
    }
}
