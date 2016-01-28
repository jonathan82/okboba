using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.Models
{
    public class UnreadConversationModel
    {
        public ConversationMap Map { get; set; }
        public Profile OtherProfile { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}
