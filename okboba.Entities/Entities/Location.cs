using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class Location : IEquatable<Location>
    {
        [Key]
        [Column(Order = 1)]
        public Int16 LocationId1 { get; set; }
        [Key]
        [Column(Order = 2)]
        public Int16 LocationId2 { get; set; }
        public string LocationName1 { get; set; }
        public string LocationName2 { get; set; }

        public bool Equals(Location other)
        {
            if (other == null) return false;
            return other.LocationId1 == this.LocationId1;
        }

        public override int GetHashCode()
        {
            return LocationId1.GetHashCode();
        }
    }
}
