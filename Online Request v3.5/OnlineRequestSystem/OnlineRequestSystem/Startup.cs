using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineRequestSystem.Startup))]

namespace OnlineRequestSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}