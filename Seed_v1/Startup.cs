using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Seed_v1.Startup))]
namespace Seed_v1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
