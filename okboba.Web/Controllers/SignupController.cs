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
        
        
    }
}