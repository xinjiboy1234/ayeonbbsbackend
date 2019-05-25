using ayeonbbsbackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.ViewModels
{
    public class FirstCategoryViewModel: FirstCategory
    {
        public PageViewModel PageViewModel { get; set; }
        public int SecondCategoryCount { get; set; }
        public int PostCount { get; set; }
    }
}
