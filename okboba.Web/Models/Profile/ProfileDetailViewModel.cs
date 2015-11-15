using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class ProfileDetailViewModel
    {
        public ProfileText ProfileText { get; set; }
        public ProfileDetail ProfileDetail { get; set; }
        public Dictionary<string,string> DetailDict { get; set; }
        public bool isMe { get; set; }
    }
}