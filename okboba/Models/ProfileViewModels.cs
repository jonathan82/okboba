﻿using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class ProfileViewModel
    {
        public Profile Profile { get; set; }
        public ProfileText ProfileText { get; set; }
        public List<string> Photos { get; set; }
        public string StorageUrl { get; set; }

        public ProfileViewModel(Profile profile)
        {
            this.Photos = new List<string>();
            this.Profile = profile;
            this.ProfileText = profile.ProfileText;
            this.StorageUrl = ConfigurationManager.AppSettings["StorageUrl"];

            if(profile.PhotosInternal != null)
            {
                var photos =  profile.PhotosInternal.Split(';');

                foreach (var p in photos)
                {
                    this.Photos.Add(p);
                }
            }
        }
    }

}