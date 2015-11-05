using okboba.Entities;
using okboba.Repository;
using okboba.Repository.EntityRepository;
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
        IPhotoRepository _photoRepo;

        public PhotoController()
        {
            _photoRepo = EntityPhotoRepository.Instance;
        }

        // GET: Image
        public ActionResult Index()
        {
            var profile = GetUserProfile();

            var vm = new ProfileViewModel(profile);            

            return View(vm);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, int topThumb, int leftThumb, int widthThumb)
        {
            //Check if user has more than max allowed photos

            _photoRepo.UploadPhoto(upload.InputStream, leftThumb, topThumb, widthThumb, GetProfileId());

            return RedirectToAction("Index");
        }
    }
}