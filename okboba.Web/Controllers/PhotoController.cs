using Microsoft.AspNet.Identity;
using okboba.Entities;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Resources;
using okboba.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            vm.Thumbnails = profile.GetThumbnails();

            return PartialView("_ListPhotos",vm);
        }

        // GET: Image
        public ActionResult Index(string userId)
        {
            var vm = new PhotoIndexViewModel();
            
            if (string.IsNullOrEmpty(userId) || User.Identity.GetUserId()==userId)
            {
                //Viewing own profile
                vm.IsMe = true;
                vm.ProfileId = GetProfileId();
            }
            else
            {
                vm.IsMe = false;
                vm.ProfileId = _profileRepo.GetProfileId(userId);
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase upload, int topThumb, int leftThumb, int widthThumb)
        {
            var profileId = GetProfileId();

            //Check if file size too big
            if(upload.ContentLength > OkbConstants.MAX_PHOTO_SIZE)
            {
                return new HttpStatusCodeResult(400, "Photo too big");
            }

            //Check if user has more than max allowed photos
            if(_photoRepo.GetNumOfPhotos(profileId) > OkbConstants.MAX_NUM_PHOTOS)
            {
                return new HttpStatusCodeResult(400, "More than max photos");
            }

            await _photoRepo.UploadAsync(upload.InputStream, leftThumb, topThumb, widthThumb, profileId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> EditThumbnail(string photo, int topThumb, int leftThumb, int widthThumb, int photoScreenWidth)
        {
            var profileId = GetProfileId();

            await _photoRepo.EditThumbnailAsync(photo, topThumb, leftThumb, widthThumb, photoScreenWidth, profileId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string photo)
        {
            await _photoRepo.DeleteAsync(photo, GetProfileId());

            return Content("");
        }
    }
}