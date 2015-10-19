using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace okboba.Entities
{
    public class Profile
    {
        public int Id { get; set; }

        [StringLength(10)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime Birthdate { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }

        public Int16 Height { get; set; }
        public Int16 LocationId1 { get; set; }
        public Int16 LocationId2 { get; set; }

        [StringLength(90)]
        public string PhotosInternal { get; set; } //list of semicolon separated filenames

        [ForeignKey("CurrentQuestion")]
        public Int16? CurrentQuestionId { get; set; }

        //Navigation properties
        public virtual Location Location { get; set; }
        public virtual ProfileText ProfileText { get; set; }
        public Question CurrentQuestion { get; set; }
    }

    public class Question
    {
        public Int16 Id { get; set; }
        [StringLength(255)]
        public string Text { get; set; }
        public int Rank { get; set; }
        public Int16? NextQuestionId { get; set; }
        public string ChoicesInternal { get; set; } //semicolon delimited array of strings
        //public string TraitScoresInternal { get; set; } //semicolon delimited array of integers representing trait score for each answer choice
        [MaxLength(10)]
        public byte[] TraitScores { get; set; }
        [ForeignKey("Trait")]
        public Int16? TraitId { get; set; }
        
        // Convenience properties for view model
        public List<string> Choices { get; set; }

        // Navigation properties
        public virtual Trait Trait { get; set; }
    }

    public class Trait
    {
        public Int16 Id { get; set; }
        public string Description { get; set; }
    }

    public class Answer
    {
        [Key]
        [Column(Order =1)]
        public int ProfileId { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int16 QuestionId { get; set; }

        public byte ChoiceIndex { get; set; }
        public byte ChoiceWeight { get; set; }
        public byte ChoiceAcceptable { get; set; }

        [Column(TypeName ="smalldatetime")] //SQL Server
        //[Column(TypeName = "timestamp")] //MySQL 
        public DateTime LastAnswered { get; set; }

        //References
        public virtual Profile Profile { get; set; }
        public virtual Question Question { get; set; }
    }

    public class Location
    {
        [Key]
        [Column(Order = 1)]
        public Int16 LocationId1 { get; set; }
        [Key]
        [Column(Order = 2)]
        public Int16 LocationId2 { get; set; }
        public string LocationName1 { get; set; }
        public string LocationName2 { get; set; }
    }

    public class ProfileText
    {
        [Key, ForeignKey("Profile")]
        public int ProfileId { get; set; }
        public string Question1 { get; set; }
        public string Question2 { get; set; }
        public string Question3 { get; set; }
        public string Question4 { get; set; }
        public string Question5 { get; set; }

        //Navigation property
        public virtual Profile Profile { get; set; }
    }

    //public class ProfileImage
    //{
    //    [Key]
    //    [StringLength(12)]
    //    public string ImageFilename { get; set; } 

    //    [ForeignKey("Profile")]
    //    public int ProfileId { get; set; }
                
    //    public Int16 LeftThumb { get; set; } 
    //    public Int16 TopThumb { get; set; } 
    //    public Int16 WidthThumb { get; set; }  

    //    //Navigation
    //    public virtual Profile Profile { get; set; }
    //}
}
