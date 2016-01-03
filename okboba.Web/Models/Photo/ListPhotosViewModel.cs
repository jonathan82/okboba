using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class ListPhotosViewModel
    {
        //public int ProfileId { get; set; }
        public string UserId { get; set; }
        public IEnumerable<Photo> Thumbnails { get; set; }
        public bool IsMe { get; set; }
    }
}