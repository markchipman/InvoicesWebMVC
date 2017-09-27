using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebInvoicesMVC.Startup))]
namespace WebInvoicesMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
