using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace clinic.Models
{
    public class ReviewViewModel
    {
        public int ReviewId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Title { get; set; }
        public int? Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime? ReviewDate { get; set; }
        public List<CommnentViewModel> Comments { get; set; }
    }
}