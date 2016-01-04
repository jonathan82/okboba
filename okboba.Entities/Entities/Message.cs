using Newtonsoft.Json;
using okboba.Resources;
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

        [StringLength(OkbConstants.MAX_MESSAGE_LENGTH)]
        public string MessageText { get; set; }

        public DateTime Timestamp { get; set; }

        [ForeignKey("Conversation")]
        public int ConversationId { get; set; }

        [ForeignKey("FromProfile")]
        public int From { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual Conversation Conversation { get; set; }
        [JsonIgnore]
        public virtual Profile FromProfile { get; set; }
    }
}
