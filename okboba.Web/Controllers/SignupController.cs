using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using okboba.Entities;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace okboba.Web.Controllers
{
    public class SignupController : Controller
    {
        private ILocationRepository _locationRepo;

        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        public SignupController()
        {
            _locationRepo = EntityLocationRepository.Instance;
        }
        
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var provList = _locationRepo.GetProvinces();
            //var provinceList = new List<object>();

            //foreach (var loc in locationList)
            //{
            //    provinceList.Add(new { id = loc.LocationId1, name = loc.LocationName1 });
            //}

            var json = JsonConvert.SerializeObject(provList);

            ViewBag.JsonProvinces = json;

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new OkbUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var loc = new Location
                {
                    LocationId1 = model.LocationId1,
                    LocationId2 = model.LocationId2
                };

                var userProfile = new Profile
                {
                    Gender = model.Gender,
                    Birthdate = model.Birthdate,
                    Name = model.Name,
                    LocationId1 = model.LocationId1,
                    LocationId2 = model.LocationId2
                };

                user.Profile = userProfile;

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }

                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}