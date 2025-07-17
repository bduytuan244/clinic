using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace clinic.Models
{
    public class Talk
    {
        public int chat_id { get; set; }
        public int sender_id { get; set; }
        public int receiver_id { get; set; }
        public string sender_role { get; set; }
        public string receiver_role { get; set; }
        public string message_text { get; set; }
        public DateTime sent_at { get; set; }
        public bool is_read { get; set; }
    }
}