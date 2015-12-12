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

        [StringLength(100)]
        public string Subject { get; set; }
    }
}
