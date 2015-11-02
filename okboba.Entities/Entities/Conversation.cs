using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class Conversation
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime? LastMessageDate { get; set; }
        [StringLength(255)]
        public string LastMessageBlurb { get; set; }
        [ForeignKey("Profile")]
        public int? LastMessageFrom { get; set; }

        //Navigation properties
        public virtual Profile Profile { get; set; }
    }
}
