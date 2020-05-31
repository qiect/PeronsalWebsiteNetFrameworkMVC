using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PersonalSite.Startup))]
namespace PersonalSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
