﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class Profile
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime Birthdate { get; set; }

        public byte Gender { get; set; }

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

        //Helper methods
        public string[] GetPhotos()
        {
            if (string.IsNullOrEmpty(PhotosInternal)) return new string[0];
            return PhotosInternal.Split(';');
        }

        public string GetFirstPhoto()
        {
            if (string.IsNullOrEmpty(PhotosInternal)) return "";
            var index = PhotosInternal.IndexOf(';');
            if (index == -1) index = PhotosInternal.Length;
            return PhotosInternal.Substring(0, index);
        }

        public int GetAge()
        {
            DateTime today = DateTime.Today;
            int age = today.Year - Birthdate.Year;
            if (Birthdate > today.AddYears(-age)) age--;
            return age;
        }
    }
}
