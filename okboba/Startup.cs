using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(okboba.Startup))]
namespace okboba
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
