using ayeonbbsbackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.ViewModels
{
    public class PostInfoViewModel : PostInfo
    {
        public PageViewModel PageViewModel { get; set; }
        public int ReplyCount { get; set; }
        public int PostGoodCount { get; set; }
        public PostGood PostGood { get; set; }
        public List<int> SecondCategoryIds { get; set; }
    }

    public class PostInfoViewModelResponse
    {
        public List<PostInfoViewModel> PostInfos { get; set; }
        public int TotalCount { get; set; }
    }
}