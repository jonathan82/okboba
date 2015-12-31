﻿using okboba.Models.Validation;
using okboba.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public byte Gender { get; set; }

        [Required]
        public byte LookingForGender { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        public string Nickname { get; set; }

        [Required]
        public Int16 LocationId1 { get; set; }

        [Required]
        public Int16 LocationId2 { get; set; }

        public string JsonProvinces { get; set; }
    }
}