using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IMSmvc.Startup))]
namespace IMSmvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}