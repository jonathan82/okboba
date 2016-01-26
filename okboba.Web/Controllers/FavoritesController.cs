using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Web.Controllers
{
    [Authorize]
    public class FavoritesController : OkbBaseController
    {
        private IFavoriteRepository _favRepo;
        private ILocationRepository _locRepo;

        public FavoritesController()
        {
            _favRepo = EntityFavoriteRepository.Instance;
            _locRepo = EntityLocationRepository.Instance;
        }

        // GET: Favorites
        public ActionResult Index()
        {
            var matchClient = GetMatchApiClient();

            var me = GetMyProfileId();

            var favorites = _favRepo.GetFavorites(me);

            var vm = new List<FavoriteViewModel>();

            //caclulate the match scores in real-time
            foreach (var fav in favorites)
            {
                var match = matchClient.CalculateMatchAsync(fav.Id).Result;

                var model = new FavoriteViewModel
                {
                    FavoriteProfile = fav,
                    MatchInfo = match
                };

                model.FavoriteProfile.LocationSring = _locRepo.GetLocationString(fav.LocationId1, fav.LocationId2);

                vm.Add(model);                
            }

            return View(vm);
        }

        /// <summary>
        /// API: Favorite a person
        /// </summary>
        [HttpPost]
        public JsonResult Save(int favoriteId)
        {
            var me = GetMyProfileId();

            _favRepo.Save(me, favoriteId);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API: Remove a user from favorites list
        /// </summary>
        [HttpPost]
        public JsonResult Remove(int favoriteId)
        {
            var me = GetMyProfileId();

            _favRepo.Remove(me, favoriteId);

            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}