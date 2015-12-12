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

        [ForeignKey("OtherProfile")]
        public int Other { get; set; }

        //Navigation properties
        public virtual Conversation Conversation { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual Profile OtherProfile { get; set; }
        public virtual Message LastMessage { get; set; }
    }
}
