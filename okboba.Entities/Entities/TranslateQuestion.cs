using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class TranslateQuestion
    {
        public Int16 Id { get; set; }

        [StringLength(255)]
        public string QuesEng { get; set; }

        [StringLength(255)]
        public string QuesChin { get; set; }

        public int Rank { get; set; }

        public string ChoicesInternalChin { get; set; } //semicolon delimited array of strings       
        public string ChoicesInternalEng { get; set; } //semicolon delimited array of strings       

        [MaxLength(10)]
        public byte[] TraitScores { get; set; }

        [ForeignKey("Trait")]
        public Int16? TraitId { get; set; }

        public bool Delete { get; set; }

        // Navigation properties
        public virtual Trait Trait { get; set; }
    }
}
