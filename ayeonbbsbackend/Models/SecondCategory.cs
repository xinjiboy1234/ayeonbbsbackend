using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Models
{
    public class SecondCategory
    {
        public int SecondCategoryId { get; set; }
        public string SecondCategoryName { get; set; }
        public int Status { get; set; }
        public FirstCategory FirstCategory { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public List<PostInfo> PostInfos { get; set; }
    }
}
