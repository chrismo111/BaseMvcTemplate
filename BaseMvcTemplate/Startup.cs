using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BaseMvcTemplate.Startup))]
namespace BaseMvcTemplate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
