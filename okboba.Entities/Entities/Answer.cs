using Newtonsoft.Json;
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
    /// 
    /// Note: we have a composite primary key consisting of ProfileId and QuestionId.  For performance this may
    ///       need to be removed and we just treat the table as a "heap".
    /// </summary>
    public class Answer
    {
        [Key]
        [Column(Order = 1)]
        public int ProfileId { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int16 QuestionId { get; set; }

        public byte? ChoiceIndex { get; set; }
        public byte ChoiceWeight { get; set; }
        public byte ChoiceAccept { get; set; }

        [Column(TypeName = "smalldatetime")] //SQL Server
        //[Column(TypeName = "timestamp")] //MySQL 
        public DateTime LastAnswered { get; set; }

        //References
        [JsonIgnore]
        public virtual Profile Profile { get; set; }
        [JsonIgnore]
        public virtual Question Question { get; set; }   
        
        //Helper functions
        public byte ChoiceBit()
        {
            return ChoiceIndex != null ? (byte)(1 << (ChoiceIndex - 1)) : (byte)0;
        }

        /// <summary>
        /// Returns true if the other answer matches my requirements
        /// </summary>
        public bool IsMatch(Answer otherAnswer)
        {
            return (otherAnswer.ChoiceBit() & ChoiceAccept) != 0;
        }

        public bool IsMatch(int index)
        {
            return ((1 << (index - 1)) & ChoiceAccept) != 0;
        }
    }
}
