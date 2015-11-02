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
        public string ChoicesInternal { get; set; } //semicolon delimited array of strings       
        [MaxLength(10)]
        public byte[] TraitScores { get; set; }
        [ForeignKey("Trait")]
        public Int16? TraitId { get; set; }

        // Convenience properties for view model
        public List<string> Choices { get; set; }

        // Navigation properties
        public virtual Trait Trait { get; set; }
    }
}
