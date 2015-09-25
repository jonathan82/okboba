using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace okboba.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }

        [StringLength(10)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime Birthdate { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }

        public Int16 Height { get; set; }

        [StringLength(15)]
        public string Location { get; set; }
    }

    public class Question
    {
        public Int16 Id { get; set; }
        [StringLength(255)]
        public string Text { get; set; }
        public int Rank { get; set; }
        public string Choices { get; set; } //semicolon delimited array of strings
        //public string TraitScores { get; set; } //semicolon delimited array of integers representing trait score for each answer choice

        //References
        //public virtual TraitModel Trait { get; set; }
    }

    public class TraitModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class UserAnswer
    {
        [Key]
        [Column(Order =1)]
        public int UserProfileId { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int16 QuestionId { get; set; }

        public byte ChoiceIndex { get; set; }
        public byte ChoiceWeight { get; set; }
        public byte ChoiceAcceptable { get; set; }

        [Column(TypeName ="smalldatetime")]
        public DateTime LastAnswered { get; set; }

        //References
        public virtual UserProfile UserProfile { get; set; }
        public virtual Question Question { get; set; }
    }
}
