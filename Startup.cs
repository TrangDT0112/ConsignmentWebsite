using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ConsignmentWebsite.Startup))]
namespace ConsignmentWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
