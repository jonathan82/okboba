using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class SettingsViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string LocationString { get; set; }

        [Required]
        public int LocationId1 { get; set; }

        [Required]
        public int LocationId2 { get; set; }

        public string Language { get; set; }

        public IList<LocationPinyinModel> Provinces { get; set; }
    }
}