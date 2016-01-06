using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class ReplyViewModel
    {
        public int ConversationId { get; set; }
        public Profile Me { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public Profile Other { get; set; }
    }
}