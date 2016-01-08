using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Security.Cookies;
using System.Configuration;
using okboba.Repository.RedisRepository;

[assembly: OwinStartup(typeof(okboba.Instant.Startup))]

namespace okboba.Instant
{
    public class Startup
    {
        public const string IDENTITY_COOKIE_NAME = "OkbobaCookie";

        public void Configuration(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                //LoginPath = new PathString("/Account/Login"),
                //Provider = new CookieAuthenticationProvider
                //{
                //    // Enables the application to validate the security stamp when the user logs in.
                //    // This is a security feature which is used when you change a password or add an external login to your account.  
                //    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, OkbUser>(
                //        validateInterval: TimeSpan.FromMinutes(30),
                //        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager, DefaultAuthenticationTypes.ApplicationCookie))
                //},
                CookieDomain = ".okboba.com",
                CookieName = IDENTITY_COOKIE_NAME
            });

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            //app.MapSignalR();
            // Branch the pipeline here for requests that start with "/signalr"
            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });

            // Create singleton Redis connection object - used by message repository
            var redisConnStr = ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString;
            SXGenericRepository.Create(redisConnStr);
        }
    }
}
