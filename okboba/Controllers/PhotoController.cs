using okboba.Entities;
using okboba.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Controllers
{
    [Authorize]
    public class PhotoController : OkbBaseController
    {
        // GET: Image
        public ActionResult Index()
        {
            //List all the photos
            OkbDbContext db = new OkbDbContext();

            var profileId = GetProfileId();

            var results = from photo in db.ProfileImages.AsQueryable()
                          where photo.ProfileId == profileId
                          select photo;

            var vm = new PhotoViewModel();
            vm.ContentUrl = "/Content/user_photos/";

            foreach(var p in results)
            {
                vm.Photos.Add(p);
            }

            return View(vm);
        }
    }
}