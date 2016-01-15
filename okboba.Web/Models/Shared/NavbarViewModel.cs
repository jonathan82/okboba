using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class NavbarViewModel
    {
        public int UnreadCount { get; set; }
        public Profile MyProfile { get; set; }
        public bool HasPhoto { get; set; }
        public int NumQuesAnswered { get; set; }
        public bool HasProfileText { get; set; }
    }
}