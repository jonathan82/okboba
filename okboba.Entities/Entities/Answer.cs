using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    /// <summary>
    /// There are 3 cases that the application must handle with regards to a user answering
    /// questions. 
    /// 
    /// Case 1: The user has answered the question and chose an acceptable respones.  This implies
    ///         that they also chose an importance ranking for the question of ChoiceWeight = 1, 2 or 3
    /// 
    /// Case 2: The user has answered the question and chose "Irrelevant" for the importance.  This
    ///         is also the same as choosing every answer as acceptable. In this case the other user's answer
    ///         doesn't factor into the match percentage, and ChoiceWeight = 0.
    /// 
    /// Case 3: The user has skipped the question.  We want to keep track of this so we'll use
    ///         ChoiceIndex = null to indicate skipped question.
    /// 
    /// Case 4: The user hasn't answered the question. In this case no answer will appear in the Answers table.
    /// </summary>
    public class Answer
    {
        [Key]
        [Column(Order = 1)]
        public int ProfileId { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int16 QuestionId { get; set; }

        public byte? ChoiceBit { get; set; }
        public byte ChoiceWeight { get; set; }
        public byte ChoiceAcceptable { get; set; }

        [Column(TypeName = "smalldatetime")] //SQL Server
        //[Column(TypeName = "timestamp")] //MySQL 
        public DateTime LastAnswered { get; set; }

        //References
        public virtual Profile Profile { get; set; }
        public virtual Question Question { get; set; }
    }
}
