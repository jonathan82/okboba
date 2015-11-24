using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class QuestionChoice
    {
        [Key]
        [ForeignKey("Questions")]
        [Column(Order = 1)]
        public Int16 QuestionId { get; set; }

        [Key]
        [Column(Order = 2)]
        public byte Index { get; set; }

        [StringLength(50)]
        public string Text { get; set; }

        public Int16 Score { get; set; }

        //navigation properties
        public Question Questions { get; set; }
    }
}
