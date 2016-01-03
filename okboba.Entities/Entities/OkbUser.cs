using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using okboba.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class OkbUser : IdentityUser
    {
        public OkbUser()
        {
            this.Id = OkbUuid.GenerateUserId();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<OkbUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public DateTime JoinDate { get; set; }

        [ForeignKey("Profile")]
        public int? ProfileId { get; set; }

        //Navigation properties
        public virtual Profile Profile { get; set; }

    }
}
