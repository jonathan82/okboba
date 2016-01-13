using Microsoft.AspNet.Identity;
using okboba.Entities;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Resources;
using okboba.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private IActivityRepository _feedRepo;

        public PhotoController()
        {
            _photoRepo = EntityPhotoRepository.Instance;
            _profileRepo = EntityProfileRepository.Instance;
            _feedRepo = EntityActivityRepository.Instance;
        }

        [ChildActionOnly]
        public ActionResult ListPhotos(string userId, bool isMe)
        {
            var vm = new ListPhotosViewModel();

            var profile = _profileRepo.GetProfile(userId);

            vm.UserId = userId;
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
                vm.UserId = User.Identity.GetUserId();
            }
            else
            {
                vm.IsMe = false;
                vm.ProfileId = _profileRepo.GetProfileId(userId);
                vm.UserId = userId;
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase upload, int topThumb, int leftThumb, int widthThumb, int photoScreenWidth)
        {
            var me = GetProfileId();
            var userId = User.Identity.GetUserId();

            //Check if file size too big
            if(upload.ContentLength > OkbConstants.MAX_PHOTO_SIZE)
            {
                return new HttpStatusCodeResult(400, "Photo too big");
            }

            //Check if user has more than max allowed photos
            if(_photoRepo.GetNumOfPhotos(me) > OkbConstants.MAX_NUM_PHOTOS)
            {
                return new HttpStatusCodeResult(400, "More than max photos");
            }

            string photo = "";
            try
            {
                photo = await _photoRepo.UploadAsync(upload.InputStream, leftThumb, topThumb, widthThumb, photoScreenWidth, me, userId);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            

            //Update activity feed
            if (IsOkToAddActivity(OkbConstants.ActivityCategories.UploadedPhoto))
            {
                _feedRepo.UploadPhotoActivity(me, photo);
                UpdateActivityLastAdded(OkbConstants.ActivityCategories.UploadedPhoto);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> EditThumbnail(string photo, int topThumb, int leftThumb, int widthThumb, int photoScreenWidth)
        {
            var userId = User.Identity.GetUserId();

            await _photoRepo.EditThumbnailAsync(photo, topThumb, leftThumb, widthThumb, photoScreenWidth, userId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string photo)
        {
            var me = GetProfileId();
            var userId = User.Identity.GetUserId();

            await _photoRepo.DeleteAsync(photo, me, userId);

            return Content("");
        }
    }
}