using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class ProfileViewModel
    {
        public Profile Profile { get; set; }
        public ProfileText ProfileText { get; set; }
    }
}