using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace clinic.Models
{
    public class Review
    {
        public int review_id { get; set; }
        public int patient_id { get; set; }
        public int doctor_id { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public DateTime created_at { get; set; }

        public virtual customer Customer { get; set; }
    }
}