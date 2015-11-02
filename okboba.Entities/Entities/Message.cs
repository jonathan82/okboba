using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class Message
    {
        public int Id { get; set; }
        [ForeignKey("Conversation")]
        public int ConversationId { get; set; }
        [ForeignKey("FromProfile")]
        public int FromProfileId { get; set; }
        [StringLength(1000)]
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }

        // Navigation properties
        public Conversation Conversation { get; set; }
        public Profile FromProfile { get; set; }
    }
}
