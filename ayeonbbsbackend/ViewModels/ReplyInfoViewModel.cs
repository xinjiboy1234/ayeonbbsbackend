using ayeonbbsbackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.ViewModels
{
    public class ReplyInfoViewModel : ReplyInfo
    {
        public PageViewModel PageViewModel { get; set; }
        public List<ReplyInfoViewModel> ReplyInfoViewModels { get; set; }
        public int InnerReplyCount { get; set; }
        public int ReplyGoodCount { get; set; }
        public ReplyGood ReplyGood { get; set; }
        /// <summary>
        /// 被回复用户
        /// </summary>
        public UserInfo RepliedUserInfo { get; set; }
    }

    public class ReplyInfoViewModelResponse
    {
        public List<ReplyInfoViewModel> ReplyInfoViewModels { get; set; }
        public int ReplyTotalCount { get; set; }
    }
}
