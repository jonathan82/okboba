using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class ProfileViewModel
    {
        public bool IsMe { get; set; }
        public int ProfileId { get; set; }
    }
    //public class ProfileViewModel
    //{
    //    // Common to all pages
    //    public Profile Profile { get; set; }
    //    public List<string> Photos { get; set; }
    //    public string StorageUrl { get; set; }

    //    // Profile Main page
    //    public ProfileText ProfileText { get; set; }

    //    // Question page
    //    public Question CurrentQuestion { get; set; }

    //    public ProfileViewModel(Profile profile)
    //    {
    //        this.Photos = new List<string>();
    //        this.Profile = profile;
    //        this.ProfileText = profile.ProfileText;
    //        this.StorageUrl = ConfigurationManager.AppSettings["StorageUrl"];

    //        if(profile.PhotosInternal != null)
    //        {
    //            var photos =  profile.PhotosInternal.Split(';');

    //            foreach (var p in photos)
    //            {
    //                this.Photos.Add(p);
    //            }
    //        }
    //    }
    //}

}