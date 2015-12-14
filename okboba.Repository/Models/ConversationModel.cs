using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.Models
{
    public class ConversationModel
    {
        public Profile OtherProfile { get; set; }
        public Message LastMessage { get; set; }
        public bool HasBeenRead { get; set; }
    }
}
