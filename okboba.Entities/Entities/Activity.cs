﻿using okboba.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    /*
        user joined
        user uploaded photo
        user edited profile
        user answered question    
    */
    public class ActivityCategory
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class Activity
    {
        public int Id { get; set; }        
        public int CategoryId { get; set; }
        public int Who { get; set; }

        [StringLength(OkbConstants.FEED_BLURB_SIZE)]
        public string Field1 { get; set; }

        [StringLength(OkbConstants.FEED_BLURB_SIZE)]
        public string Field2 { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
