using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SistemaGestion.Startup))]
namespace SistemaGestion
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
