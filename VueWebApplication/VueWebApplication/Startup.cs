using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VueWebApplication.Startup))]
namespace VueWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
