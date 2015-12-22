using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class Question
    {
        public Int16 Id { get; set; }

        [StringLength(255)]
        public string Text { get; set; }

        public int Rank { get; set; }

        [ForeignKey("Trait")]
        public Int16? TraitId { get; set; }

        // Navigation properties
        public virtual Trait Trait { get; set; }
    }
}
