using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class Favorite
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Profile")]
        public int ProfileId { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("FavoriteProfile")]
        public int FavoriteId { get; set; }

        public DateTime FavoriteDate { get; set; }

        //navigation properties
        public virtual Profile Profile { get; set; }
        public virtual Profile FavoriteProfile { get; set; }

    }
}
