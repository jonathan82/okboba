using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class ProfileViewModel
    {
        public bool IsMe { get; set; }
        public int ProfileId { get; set; }
    }
    
}