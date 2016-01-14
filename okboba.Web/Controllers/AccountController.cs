using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using okboba.Web.Models;
using okboba.Entities;
using System.Collections.Generic;
using okboba.Repository;
using Newtonsoft.Json;
using okboba.Repository.EntityRepository;
using System.Net;
using okboba.Resources;

//some comments

namespace okboba.Web.Controllers
{
    [Authorize]
    public class AccountController : OkbBaseController
    {
        private ILocationRepository _locationRepo;
        private IActivityRepository _feedRepo;

        public AccountController()
        {
            _locationRepo = EntityLocationRepository.Instance;
            _feedRepo = EntityActivityRepository.Instance;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //
        // GET: /Account/VerifyEmail
        [AllowAnonymous]
        public JsonResult VerifyEmail(string email)
        {
            var user = UserManager.FindByEmail(email);

            if (user != null)
            {
                //email already taken
                return Json(i18n.Error_EmailTaken, JsonRequestBehavior.AllowGet);
            }

            //email OK
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = "/home")
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Content("fail");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    RememberLastLogin(model.Email);              
                    return Content("success");

                case SignInStatus.Failure:
                default:
                    Response.StatusCode = 400;
                    return Content("fail");
            }
        }
                                        
        //
        // POST: /Account/LogOff
        [HttpGet]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //return RedirectToAction("Index", "Home");
            Session.Abandon();
            return Redirect("/");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
            {
                return Redirect("/home");
            }

            var provList = _locationRepo.GetProvinces();

            var json = JsonConvert.SerializeObject(provList);

            var vm = new RegisterViewModel();
            vm.JsonProvinces = json;

            //ViewBag.JsonProvinces = json;

            return View(vm);
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
                    Email = model.Email,
                    JoinDate = DateTime.Now,
                    LastLoginDate = DateTime.Now
                };

                var profile = new Profile
                {
                    Gender = model.Gender,
                    LookingForGender = model.LookingForGender,
                    Birthdate = model.Birthdate,
                    Nickname = model.Nickname,
                    LocationId1 = model.LocationId1,
                    LocationId2 = model.LocationId2
                };

                user.Profile = profile;

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
            // Re-populate model with provinces           
            var prov = _locationRepo.GetProvinces();
            model.JsonProvinces = JsonConvert.SerializeObject(prov);
            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void RememberLastLogin(string email)
        {
            var user = UserManager.FindByEmail(email);
            user.LastLoginDate = DateTime.Now;
            UserManager.Update(user);
        }
        
        #endregion
    }
}