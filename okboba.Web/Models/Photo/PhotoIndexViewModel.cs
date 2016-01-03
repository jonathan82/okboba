using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class PhotoIndexViewModel
    {
        public string UserId { get; set; }
        public int ProfileId { get; set; }
        public bool IsMe { get; set; }
    }
}