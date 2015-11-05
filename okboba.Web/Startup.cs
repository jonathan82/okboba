using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(okboba.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

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
