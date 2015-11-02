using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class Trait
    {
        public Int16 Id { get; set; }
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
    }
}
