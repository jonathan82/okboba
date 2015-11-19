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
        private IPhotoRepository _photoRepo;
        private IProfileRepository _profileRepo;

        public PhotoController()
        {
            _photoRepo = EntityPhotoRepository.Instance;
            _profileRepo = EntityProfileRepository.Instance;
        }

        [ChildActionOnly]
        public ActionResult ListPhotos(int id, bool isMe)
        {
            var vm = new ListPhotosViewModel();

            var profile = _profileRepo.GetProfile(id);

            vm.ProfileId = id;
            vm.IsMe = isMe;
            vm.Photos = profile.GetPhotos();

            return PartialView("_ListPhotos",vm);
        }

        // GET: Image
        public ActionResult Index(int? id)
        {
            var vm = new PhotoIndexViewModel();
            
            if (id == null)
            {
                //Viewing own profile
                vm.IsMe = true;
                vm.ProfileId = GetProfileId();
            }
            else
            {
                vm.IsMe = false;
                vm.ProfileId = (int)id;
            }

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