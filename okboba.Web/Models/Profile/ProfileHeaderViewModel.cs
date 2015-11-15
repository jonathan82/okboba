using okboba.Entities;
using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class ProfileHeaderViewModel
    {
        public Profile Profile { get; set; }
        public MatchModel Match { get; set; }
        public bool IsMe { get; set; }
        public string Section { get; set; }
    }
}