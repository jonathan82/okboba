using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.Models
{
    public class ActivityModel
    {
        public Activity Activity { get; set; }
        public Profile Profile { get; set; }
    }
}
