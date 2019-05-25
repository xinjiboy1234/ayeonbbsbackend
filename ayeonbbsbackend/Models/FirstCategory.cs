using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Models
{
    public class FirstCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FirstCategoryId { get; set; }
        public string FirstCategoryName { get; set; }
        public int Status { get; set; }
        public List<SecondCategory> SecondCategories { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
