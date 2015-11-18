using System;
using System.Collections.Generic;
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
        public int ProfileId { get; set; }
        public string Metadata { get; set; }
        public DateTime When { get; set; }
    }
}
