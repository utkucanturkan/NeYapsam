using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebNeYapsam.Startup))]
namespace WebNeYapsam
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
