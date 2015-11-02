using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class ConversationMap
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Profile")]
        public int ProfileId { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Conversation")]
        public int ConversationId { get; set; }

        [ForeignKey("ToProfile")]
        public int? ToProfileId { get; set; }

        [StringLength(10)]
        public string ToPhoto { get; set; }

        [StringLength(10)]
        public string ToName { get; set; }

        public bool HasBeenRead { get; set; }
        public bool HasReplies { get; set; }

        //Navigation properties
        public Conversation Conversation { get; set; }
        public Profile Profile { get; set; }
        public Profile ToProfile { get; set; }
    }
}
